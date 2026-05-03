using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Contracts.Signals;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Exceptions;
using RestaurantSystem.Application.Integrations;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IOrderDepartmentProgressRepository _orderDepartmentProgressRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;
        private readonly IOrderNotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly ISendyIntegrationService _deliveryIntegrationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IMenuRepository menuRepository,
            IOrderDepartmentProgressRepository orderDepartmentProgressRepository,
            ITableRepository tableRepository,
            IMapper mapper,
            IOrderNotificationService notificationService,
            IInventoryService inventoryService,
            ISendyIntegrationService deliveryIntegrationService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
            _orderDepartmentProgressRepository = orderDepartmentProgressRepository;
            _tableRepository = tableRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _inventoryService = inventoryService;
            _deliveryIntegrationService = deliveryIntegrationService;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            _logger.LogInformation("📝 بدء إنشاء طلب جديد للعميل: {Phone}", request.CustomerPhoneNumber);

            await ValidateTableForDineInOrderAsync(request.OrderType, request.TableId);

            var allOrders = await _orderRepository.GetAllAsync();
            int nextOrderNumber = (allOrders.Any() ? allOrders.Max(o => o.OrderNumber) : 0) + 1;

            var order = new Order
            {
                OrderNumber = nextOrderNumber,
                UserId = request.UserId,
                TableId = request.TableId,
                OrderType = request.OrderType,
                SpecialNotes = request.SpecialNotes ?? string.Empty,
                DeliveryFee = request.DeliveryFee,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DeliveryAddress = request.DeliveryAddress,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ExternalDeliveryStatus = DeliveryPartnerStatus.Idle,
                IsSyncedToExternalProvider = false,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in request.Items)
            {
                var menuItem = await _menuRepository.GetByIdAsync(item.MenuItemId);
                if (menuItem == null)
                    throw new Exception($"المنتج {item.MenuItemId} غير موجود");

                if (!menuItem.IsAvailable)
                    throw new Exception($"المنتج {menuItem.Name} غير متاح حالياً");

                if (menuItem.DepartmentId == Guid.Empty)
                    throw new Exception($"المنتج {menuItem.Name} غير مربوط بأي قسم");

                order.OrderItems.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    DepartmentId = menuItem.DepartmentId,
                    Quantity = item.Quantity,
                    Price = menuItem.Price,
                    SpecialInstructions = item.SpecialInstructions ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                });
            }

            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity) + (order.DeliveryFee ?? 0);

            await _orderRepository.AddAsync(order);

            var distinctDepartmentIds = order.OrderItems
                .Where(x => x.DepartmentId != Guid.Empty)
                .Select(x => (Guid)x.DepartmentId)
                .Distinct()
                .ToList();

            foreach (var departmentId in distinctDepartmentIds)
            {
                await _orderDepartmentProgressRepository.AddAsync(new OrderDepartmentProgress
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    DepartmentId = departmentId,
                    Status = OrderDepartmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _orderDepartmentProgressRepository.SaveChangesAsync();

            await MarkTableOccupiedForDineInOrderAsync(order);

            var response = await GetOrderByIdAsync(order.Id);

            await _notificationService.NotifyNewOrderAsync(response);

            var grouped = order.OrderItems.GroupBy(x => x.DepartmentId);

            foreach (var group in grouped)
            {
                var departmentId = group.Key.ToString();

                var items = group.Select(i => new
                {
                    i.MenuItemId,
                    i.Quantity,
                    i.Price,
                    i.SpecialInstructions
                }).ToList();

                await _notificationService.NotifyDepartmentAsync(departmentId, new
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Items = items
                });

                _logger.LogInformation(
                    "🚀 تم إرسال {Count} عناصر إلى القسم {DeptId} للطلب #{OrderNo}",
                    items.Count,
                    departmentId,
                    order.OrderNumber);
            }

            return response;
        }

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequestDto request)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            if (request.NewStatus == OrderStatus.Confirmed && !order.IsStockDeducted)
            {
                try
                {
                    await _inventoryService.ProcessOrderStockDeductionAsync(order.Id);
                    _logger.LogInformation("✅ تم استقطاع المكونات بنجاح للطلب #{OrderNo}", order.OrderNumber);
                }
                catch (ValidationException vex)
                {
                    _logger.LogWarning(
                        "⚠️ فشل استقطاع المخزن للطلب #{OrderNo}: {Msg}",
                        order.OrderNumber,
                        vex.Message);

                    throw;
                }
            }

            // Guard: disallow completing a Sendy-synced delivery unless provider reported Delivered
            if (request.NewStatus == OrderStatus.Completed &&
                order.OrderType == OrderType.Delivery &&
                (order.IsSyncedToExternalProvider || order.ExternalOrderId.HasValue) &&
                order.ExternalDeliveryStatus != DeliveryPartnerStatus.Delivered)
            {
                throw new Exception("لا يمكن إكمال طلب توصيل مرتبط بـ Sendy قبل أن تكون حالة التوصيل Delivered");
            }

            order.Status = request.NewStatus;
            if (request.NewStatus == OrderStatus.Completed && !order.CompletedAt.HasValue)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            if (request.NewStatus == OrderStatus.Confirmed &&
                order.OrderType == OrderType.Delivery &&
                !order.IsSyncedToExternalProvider)
            {
                await ProcessExternalDeliveryPush(order);
            }

            await _orderRepository.UpdateAsync(order);

            if (IsTerminalOrderStatus(request.NewStatus))
            {
                await FreeTableIfNoActiveDineInOrdersAsync(order);
            }

            await _notificationService.NotifyOrderStatusChangedAsync(
                id,
                order.OrderNumber,
                request.NewStatus.ToString());

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<IEnumerable<OrderDepartmentProgressDto>> GetOrderDepartmentProgressAsync(Guid orderId)
        {
            var progresses = await _orderDepartmentProgressRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<OrderDepartmentProgressDto>>(progresses);
        }

        public async Task<OrderDepartmentProgressDto> UpdateOrderDepartmentStatusAsync(
            Guid orderId,
            UpdateOrderDepartmentStatusRequestDto request)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            if (order.Status == OrderStatus.Completed ||
                order.Status == OrderStatus.Cancelled ||
                order.Status == OrderStatus.Returned)
            {
                throw new Exception("لا يمكن تحديث حالة قسم لطلب مكتمل أو ملغي أو مرجع");
            }

            var progress = await _orderDepartmentProgressRepository
                .GetByOrderAndDepartmentAsync(orderId, request.DepartmentId);

            if (progress == null)
                throw new Exception("القسم غير موجود داخل هذا الطلب");

            progress.Status = request.NewStatus;
            progress.Notes = request.Notes;

            if (request.NewStatus == OrderDepartmentStatus.Preparing && !progress.StartedAt.HasValue)
            {
                progress.StartedAt = DateTime.UtcNow;
            }

            if (request.NewStatus == OrderDepartmentStatus.Ready)
            {
                progress.ReadyAt = DateTime.UtcNow;
            }
            else
            {
                progress.ReadyAt = null;
            }

            await _orderDepartmentProgressRepository.UpdateAsync(progress);
            await _orderDepartmentProgressRepository.SaveChangesAsync();

            var allProgresses = (await _orderDepartmentProgressRepository.GetByOrderIdAsync(orderId)).ToList();

            if (allProgresses.All(x => x.Status == OrderDepartmentStatus.Ready))
            {
                order.Status = OrderStatus.Ready;
            }
            else if (allProgresses.Any(x =>
                         x.Status == OrderDepartmentStatus.Preparing ||
                         x.Status == OrderDepartmentStatus.Ready))
            {
                order.Status = OrderStatus.Preparing;
            }
            else
            {
                if (order.Status != OrderStatus.Pending)
                    order.Status = OrderStatus.Confirmed;
            }

            await _orderRepository.UpdateAsync(order);

            await _notificationService.NotifyOrderStatusChangedAsync(
                order.Id,
                order.OrderNumber,
                order.Status.ToString());

            return _mapper.Map<OrderDepartmentProgressDto>(progress);
        }

        public async Task<OrderResponseDto> SyncExternalStatusAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            if (!order.ExternalOrderId.HasValue)
                return _mapper.Map<OrderResponseDto>(order);

            var (internalStatus, externalStatus, driverName, driverPhone, trackingUrl) =
                await _deliveryIntegrationService.GetDeliveryStatusAsync(order.ExternalOrderId.Value);

            ApplyExternalStatusToOrder(
                order,
                internalStatus,
                driverName,
                driverPhone,
                trackingUrl);

            await _orderRepository.UpdateAsync(order);

            if (IsTerminalOrderStatus(order.Status))
            {
                await FreeTableIfNoActiveDineInOrdersAsync(order);
            }

            await _notificationService.NotifyExternalDeliveryUpdateAsync(
                order.Id,
                order.OrderNumber,
                internalStatus,
                externalStatus);

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderResponseDto> UpdateExternalStatusFromWebhookAsync(
            Guid externalOrderId,
            string newStatus,
            string? courierName,
            string? courierPhone,
            string? trackingUrl)
        {
            var order = await _orderRepository.GetByExternalIdAsync(externalOrderId);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            var mappedStatus = MapExternalStatus(newStatus);

            ApplyExternalStatusToOrder(
                order,
                mappedStatus,
                courierName,
                courierPhone,
                trackingUrl);

            await _orderRepository.UpdateAsync(order);

            if (IsTerminalOrderStatus(order.Status))
            {
                await FreeTableIfNoActiveDineInOrdersAsync(order);
            }

            await _notificationService.NotifyExternalDeliveryUpdateAsync(
                order.Id,
                order.OrderNumber,
                mappedStatus,
                newStatus);

            return await GetOrderByIdAsync(order.Id);
        }

        private async Task ProcessExternalDeliveryPush(Order order)
        {
            _logger.LogInformation("🚀 جاري إرسال الطلب #{OrderNo} إلى Sendy...", order.OrderNumber);

            var customerName = BuildCustomerName(order);
            var customerPhone = order.CustomerPhoneNumber ?? order.User?.PhoneNumber ?? string.Empty;
            var customerAddress = order.DeliveryAddress ?? order.User?.Address ?? "No Address Provided";

            var orderValue = Math.Max(0, order.TotalAmount - (order.DeliveryFee ?? 0));
            var deliveryFee = order.DeliveryFee ?? 0;

            var paymentMethod = "cash";
            if (order.Payment != null)
            {
                paymentMethod = MapPaymentMethod(order.Payment.PaymentMethod.ToString());
            }

            var pushRequest = new IntegrationPushOrderRequest
            {
                OrderNumber = order.OrderNumber,
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                CustomerAddress = customerAddress,
                DeliveryLat = order.Latitude ?? 0,
                DeliveryLng = order.Longitude ?? 0,
                OrderValue = orderValue,
                DeliveryFee = deliveryFee,
                ExternalRef = $"ORD-{order.OrderNumber}",
                FulfillmentType = "from_to",
                DeliveryMode = "direct",
                PaymentMethod = paymentMethod
            };

            var (success, externalId, externalPublicId, trackingUrl, message) =
                await _deliveryIntegrationService.PushOrderToSendyAsync(pushRequest);

            if (success && externalId.HasValue)
            {
                var (status, statusRaw, driverName, driverPhone, statusTrackingUrl) =
                    await _deliveryIntegrationService.GetDeliveryStatusAsync(externalId.Value);

                var finalStatus = status != DeliveryPartnerStatus.Idle
                    ? status
                    : DeliveryPartnerStatus.SearchingForDriver;

                order.ExternalOrderId = externalId.Value;
                order.ExternalPublicId = externalPublicId;
                order.IsSyncedToExternalProvider = true;
                order.ExternalDeliveryStatus = finalStatus;
                order.LastExternalSyncDate = DateTime.UtcNow;
                order.TrackingUrl = statusTrackingUrl ?? trackingUrl ?? order.TrackingUrl;
                order.CourierName = string.IsNullOrWhiteSpace(driverName) ? order.CourierName : driverName;
                order.CourierPhoneNumber = string.IsNullOrWhiteSpace(driverPhone) ? order.CourierPhoneNumber : driverPhone;

                _logger.LogInformation(
                    "✅ تمت مزامنة الطلب #{OrderNo} بنجاح مع Sendy. External ID: {ExtId}, PublicId: {PublicId}, Status: {Status}, RawStatus: {RawStatus}",
                    order.OrderNumber,
                    externalId.Value,
                    externalPublicId,
                    finalStatus,
                    statusRaw);
            }
            else
            {
                _logger.LogError(
                    "❌ فشلت مزامنة الطلب #{OrderNo} مع Sendy. Message: {Msg}",
                    order.OrderNumber,
                    message);
            }
        }

        public async Task<bool> PushOrderToExternalDeliveryAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null || order.OrderType != OrderType.Delivery)
                return false;

            await ProcessExternalDeliveryPush(order);
            await _orderRepository.UpdateAsync(order);

            return order.IsSyncedToExternalProvider;
        }
        public async Task<OrderResponseDto> PushOrderToSendyAsync(Guid orderId)
        {
            _logger.LogInformation("🔁 PushOrderToSendyAsync called for order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            if (order.OrderType != OrderType.Delivery)
                throw new Exception("يمكن فقط إرسال طلبات التوصيل إلى Sendy");

            if (order.IsSyncedToExternalProvider || order.ExternalOrderId.HasValue)
                throw new Exception("تمت مزامنة هذا الطلب بالفعل مع Sendy");

            if (string.IsNullOrWhiteSpace(order.DeliveryAddress) ||
                string.IsNullOrWhiteSpace(order.CustomerPhoneNumber) ||
                !order.Latitude.HasValue ||
                !order.Longitude.HasValue)
            {
                throw new Exception("معلومات التوصيل غير مكتملة: العنوان، رقم الهاتف، خط العرض وخط الطول مطلوبة");
            }

            var progresses = order.OrderDepartmentProgresses?.ToList() ?? new List<OrderDepartmentProgress>();

            if (progresses.Any() && progresses.Any(p => p.Status != OrderDepartmentStatus.Ready))
                throw new Exception("يجب أن تكون جميع أقسام الطلب جاهزة قبل الإرسال إلى Sendy");

            var orderValue = order.OrderItems?.Sum(x => x.Price * x.Quantity) ?? 0m;

            if (orderValue <= 0)
                orderValue = Math.Max(0, order.TotalAmount - (order.DeliveryFee ?? 0));

            var paymentMethod = "cash";
            if (order.Payment != null)
                paymentMethod = MapPaymentMethod(order.Payment.PaymentMethod.ToString());

            var pushRequest = new IntegrationPushOrderRequest
            {
                OrderNumber = order.OrderNumber,
                CustomerName = BuildCustomerName(order),
                CustomerPhone = order.CustomerPhoneNumber,
                CustomerAddress = order.DeliveryAddress,
                DeliveryLat = order.Latitude.Value,
                DeliveryLng = order.Longitude.Value,
                OrderValue = orderValue,
                DeliveryFee = order.DeliveryFee ?? 0,
                ExternalRef = $"ORD-{order.OrderNumber}",
                FulfillmentType = "from_to",
                DeliveryMode = "direct",
                PaymentMethod = paymentMethod
            };

            var (success, externalId, externalPublicId, trackingUrl, message) =
                await _deliveryIntegrationService.PushOrderToSendyAsync(pushRequest);

            if (!success)
            {
                _logger.LogError(
                    "❌ Failed to push order #{OrderNumber} to Sendy. Message: {Message}",
                    order.OrderNumber,
                    message);

                throw new Exception($"فشل إرسال الطلب إلى Sendy: {message}");
            }

            if (!externalId.HasValue)
                throw new Exception("تم إرسال الطلب لكن Sendy لم يرجع ExternalOrderId");

            var (status, statusRaw, driverName, driverPhone, statusTrackingUrl) =
     await _deliveryIntegrationService.GetDeliveryStatusAsync(externalId.Value);

            var finalStatus = status != DeliveryPartnerStatus.Idle
                ? status
                : DeliveryPartnerStatus.SearchingForDriver;

            order.ExternalOrderId = externalId.Value;
            order.ExternalPublicId = externalPublicId;
            order.TrackingUrl = statusTrackingUrl ?? trackingUrl ?? order.TrackingUrl;
            order.IsSyncedToExternalProvider = true;
            order.ExternalDeliveryStatus = finalStatus;
            order.CourierName = string.IsNullOrWhiteSpace(driverName) ? order.CourierName : driverName;
            order.CourierPhoneNumber = string.IsNullOrWhiteSpace(driverPhone) ? order.CourierPhoneNumber : driverPhone;
            order.LastExternalSyncDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
      "✅ Order #{OrderNumber} pushed to Sendy successfully. ExternalId: {ExternalId}, PublicId: {PublicId}, Status: {Status}, RawStatus: {RawStatus}",
      order.OrderNumber,
      externalId.Value,
      externalPublicId,
      finalStatus,
      statusRaw);

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderResponseDto?> GetOrderByOrderNumberAsync(int orderNumber)
        {
            var orders = await _orderRepository.GetAllAsync();
            var order = orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto?> GetOrderByExternalIdAsync(Guid externalOrderId)
        {
            var order = await _orderRepository.GetByExternalIdAsync(externalOrderId);
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync() =>
            _mapper.Map<IEnumerable<OrderResponseDto>>(await _orderRepository.GetAllOrdersWithDetailsAsync());

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId) =>
            Guid.TryParse(userId, out var gId)
                ? _mapper.Map<IEnumerable<OrderResponseDto>>(await _orderRepository.GetOrdersByUserIdAsync(gId))
                : new List<OrderResponseDto>();

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByTableIdAsync(Guid tableId) =>
            _mapper.Map<IEnumerable<OrderResponseDto>>(await _orderRepository.GetOrdersByTableIdAsync(tableId));

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.ExternalOrderId.HasValue)
            {
                var cancelledInSendy = await _deliveryIntegrationService.CancelOrderAsync(
                    order.ExternalOrderId.Value,
                    "Restaurant cancelled the order");

                if (!cancelledInSendy)
                    throw new Exception("فشل إلغاء الطلب في Sendy، لم يتم إلغاء الطلب محلياً");
            }

            order.Status = OrderStatus.Cancelled;
            order.ExternalDeliveryStatus = DeliveryPartnerStatus.Cancelled;
            order.LastExternalSyncDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await FreeTableIfNoActiveDineInOrdersAsync(order);

            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(Guid orderId) =>
            (await _orderRepository.GetOrderWithDetailsAsync(orderId))?.TotalAmount ?? 0;

        public async Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync() =>
            _mapper.Map<IEnumerable<OrderResponseDto>>(await _orderRepository.GetPendingOrdersAsync());

        public async Task<OrderResponseDto> UpdateOrderAsync(Guid id, CreateOrderRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            order.SpecialNotes = request.SpecialNotes ?? string.Empty;
            order.DeliveryAddress = request.DeliveryAddress;
            order.CustomerPhoneNumber = request.CustomerPhoneNumber;
            order.Latitude = request.Latitude;
            order.Longitude = request.Longitude;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderResponseDto>(order);
        }

        private async Task ValidateTableForDineInOrderAsync(OrderType orderType, Guid? tableId)
        {
            if (orderType != OrderType.DineIn)
                return;

            if (!tableId.HasValue || tableId.Value == Guid.Empty)
                throw new Exception("يجب اختيار طاولة للطلب داخل المطعم");

            var table = await _tableRepository.GetByIdAsync(tableId.Value);
            if (table == null)
                throw new Exception("الطاولة غير موجودة");

            if (!table.IsActive)
                throw new Exception("الطاولة غير فعالة");

            if (!table.IsOrderingEnabled)
                throw new Exception("الطلب الإلكتروني غير متاح لهذه الطاولة");

            if (table.Status == TableStatus.Maintenance)
                throw new Exception("لا يمكن إنشاء طلب على طاولة تحت الصيانة");

            if (table.Status == TableStatus.Occupied)
                throw new Exception("الطاولة مشغولة حالياً");
        }

        private async Task MarkTableOccupiedForDineInOrderAsync(Order order)
        {
            if (order.OrderType != OrderType.DineIn || !order.TableId.HasValue)
                return;

            var table = await _tableRepository.GetByIdAsync(order.TableId.Value);
            if (table == null)
                throw new Exception("الطاولة غير موجودة");

            if (table.Status != TableStatus.Occupied)
            {
                table.Status = TableStatus.Occupied;
                await _tableRepository.UpdateAsync(table);

                _logger.LogInformation(
                    "🍽️ تم تغيير حالة الطاولة {TableNumber} إلى Occupied بسبب الطلب #{OrderNumber}",
                    table.TableNumber,
                    order.OrderNumber);
            }
        }

        private async Task FreeTableIfNoActiveDineInOrdersAsync(Order order)
        {
            if (order.OrderType != OrderType.DineIn || !order.TableId.HasValue)
                return;

            var tableOrders = await _orderRepository.GetOrdersByTableIdAsync(order.TableId.Value);

            var hasOtherActiveDineInOrders = tableOrders.Any(o =>
                o.Id != order.Id &&
                o.OrderType == OrderType.DineIn &&
                !IsTerminalOrderStatus(o.Status));

            if (hasOtherActiveDineInOrders)
                return;

            var table = await _tableRepository.GetByIdAsync(order.TableId.Value);
            if (table == null)
                return;

            if (table.Status == TableStatus.Occupied)
            {
                table.Status = TableStatus.Available;
                await _tableRepository.UpdateAsync(table);

                _logger.LogInformation(
                    "🟢 تم تحرير الطاولة {TableNumber} بعد إنهاء/إلغاء الطلب #{OrderNumber}",
                    table.TableNumber,
                    order.OrderNumber);
            }
        }

        private static bool IsTerminalOrderStatus(OrderStatus status)
        {
            return status == OrderStatus.Completed ||
                   status == OrderStatus.Cancelled ||
                   status == OrderStatus.Returned;
        }

        private static void ApplyExternalStatusToOrder(
            Order order,
            DeliveryPartnerStatus externalStatus,
            string? courierName,
            string? courierPhone,
            string? trackingUrl)
        {
            order.ExternalDeliveryStatus = externalStatus;
            order.CourierName = string.IsNullOrWhiteSpace(courierName) ? order.CourierName : courierName;
            order.CourierPhoneNumber = string.IsNullOrWhiteSpace(courierPhone) ? order.CourierPhoneNumber : courierPhone;
            order.TrackingUrl = string.IsNullOrWhiteSpace(trackingUrl) ? order.TrackingUrl : trackingUrl;
            order.LastExternalSyncDate = DateTime.UtcNow;

            if (externalStatus == DeliveryPartnerStatus.Delivered)
            {
                order.Status = OrderStatus.Completed;

                if (!order.CompletedAt.HasValue)
                    order.CompletedAt = DateTime.UtcNow;
            }
            else if (externalStatus == DeliveryPartnerStatus.Cancelled ||
                     externalStatus == DeliveryPartnerStatus.Failed ||
                     externalStatus == DeliveryPartnerStatus.Returned)
            {
                order.Status = OrderStatus.Cancelled;
            }
            else if (externalStatus == DeliveryPartnerStatus.PickedUp ||
          externalStatus == DeliveryPartnerStatus.InTransit ||
          externalStatus == DeliveryPartnerStatus.ArrivedAtCustomer ||
          externalStatus == DeliveryPartnerStatus.AtStore)
            {
                order.Status = OrderStatus.Delivering;
            }
            else if (externalStatus == DeliveryPartnerStatus.DriverAssigned ||
                     externalStatus == DeliveryPartnerStatus.SearchingForDriver)
            {
                order.Status = OrderStatus.ReadyForPickup;
            }
        }

        private static DeliveryPartnerStatus MapExternalStatus(string? status)
        {
            return SendyStatusMapper.MapToDeliveryPartnerStatus(status);
        }

        private static string BuildCustomerName(Order order)
        {
            var firstName = order.User?.FirstName?.Trim() ?? string.Empty;
            var lastName = order.User?.LastName?.Trim() ?? string.Empty;
            var fullName = $"{firstName} {lastName}".Trim();

            return string.IsNullOrWhiteSpace(fullName) ? "Customer" : fullName;
        }

        private static string MapPaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                return "cash";

            return paymentMethod.Trim().ToLowerInvariant() switch
            {
                "cash" => "cash",
                "creditcard" => "online",
                "debitcard" => "online",
                "zaincash" => "online",
                "wallet" => "online",
                _ => "cash"
            };
        }
    }
}
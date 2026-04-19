using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Contracts.Signals;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IOrderNotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly ISendyIntegrationService _deliveryIntegrationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IMenuRepository menuRepository,
            IMapper mapper,
            IOrderNotificationService notificationService,
            IInventoryService inventoryService,
            ISendyIntegrationService deliveryIntegrationService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _inventoryService = inventoryService;
            _deliveryIntegrationService = deliveryIntegrationService;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            _logger.LogInformation("📝 بدء إنشاء طلب جديد للعميل: {Phone}", request.CustomerPhoneNumber);

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

            try
            {
                await _inventoryService.ProcessOrderStockDeductionAsync(order.Id);
                _logger.LogInformation("✅ تم استقطاع المكونات بنجاح للطلب #{OrderNo}", order.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "⚠️ تحذير: فشل استقطاع المخزن للطلب #{OrderNo}: {Msg}",
                    order.OrderNumber,
                    ex.Message);
            }

            var response = _mapper.Map<OrderResponseDto>(order);

           // 1. إشعار عام للكاشير/المدير
await _notificationService.NotifyNewOrderAsync(response);

// 2. 🔥 توزيع الطلب حسب الأقسام (المهم)
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

    // إرسال فقط العناصر الخاصة بهذا القسم
    await _notificationService.NotifyDepartmentAsync(departmentId, new
    {
        OrderId = order.Id,
        OrderNumber = order.OrderNumber,
        Items = items
    });

    // Log حتى تتأكد يشتغل
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

            order.Status = request.NewStatus;

            if (request.NewStatus == OrderStatus.Confirmed &&
                order.OrderType == OrderType.Delivery &&
                !order.IsSyncedToExternalProvider)
            {
                await ProcessExternalDeliveryPush(order);
            }

            await _orderRepository.UpdateAsync(order);

            await _notificationService.NotifyOrderStatusChangedAsync(
                id,
                order.OrderNumber,
                request.NewStatus.ToString());

            return _mapper.Map<OrderResponseDto>(order);
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

            await _notificationService.NotifyExternalDeliveryUpdateAsync(
                order.Id,
                order.OrderNumber,
                internalStatus,
                externalStatus);

            return _mapper.Map<OrderResponseDto>(order);
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

            await _notificationService.NotifyExternalDeliveryUpdateAsync(
                order.Id,
                order.OrderNumber,
                mappedStatus,
                newStatus);

            return _mapper.Map<OrderResponseDto>(order);
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
                order.ExternalOrderId = externalId.Value;
                order.ExternalPublicId = externalPublicId;
                order.IsSyncedToExternalProvider = true;
                order.ExternalDeliveryStatus = DeliveryPartnerStatus.SearchingForDriver;
                order.LastExternalSyncDate = DateTime.UtcNow;
                order.TrackingUrl = trackingUrl;

                _logger.LogInformation(
                    "✅ تمت مزامنة الطلب #{OrderNo} بنجاح مع Sendy. External ID: {ExtId}, PublicId: {PublicId}",
                    order.OrderNumber,
                    externalId.Value,
                    externalPublicId);
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

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid id) =>
            _mapper.Map<OrderResponseDto>(await _orderRepository.GetOrderWithDetailsAsync(id));

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
                await _deliveryIntegrationService.CancelOrderAsync(
                    order.ExternalOrderId.Value,
                    "Restaurant cancelled the order");
            }

            order.Status = OrderStatus.Cancelled;
            order.ExternalDeliveryStatus = DeliveryPartnerStatus.Cancelled;
            order.LastExternalSyncDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
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
                order.CompletedAt = DateTime.UtcNow;
            }
            else if (externalStatus == DeliveryPartnerStatus.Cancelled ||
                     externalStatus == DeliveryPartnerStatus.Failed ||
                     externalStatus == DeliveryPartnerStatus.Returned)
            {
                order.Status = OrderStatus.Cancelled;
            }
            else if (externalStatus == DeliveryPartnerStatus.PickedUp ||
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
            if (string.IsNullOrWhiteSpace(status))
                return DeliveryPartnerStatus.Idle;

            return status.Trim().ToLowerInvariant() switch
            {
                "searching" or "pending" => DeliveryPartnerStatus.SearchingForDriver,
                "confirmed" or "accepted" or "assigned" => DeliveryPartnerStatus.DriverAssigned,
                "at_pickup" or "atpickup" or "arrived_at_store" => DeliveryPartnerStatus.AtStore,
                "picked_up" or "pickedup" => DeliveryPartnerStatus.PickedUp,
                "in_transit" or "intransit" => DeliveryPartnerStatus.PickedUp,
                "at_destination" or "atdestination" or "arrived" => DeliveryPartnerStatus.ArrivedAtCustomer,
                "delivered" or "completed" => DeliveryPartnerStatus.Delivered,
                "cancelled" or "canceled" or "rejected" => DeliveryPartnerStatus.Cancelled,
                "returned" => DeliveryPartnerStatus.Returned,
                "failed" or "delivery_exception" or "deliveryexception" => DeliveryPartnerStatus.Failed,
                _ => DeliveryPartnerStatus.Idle
            };
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
using Microsoft.AspNetCore.SignalR;
using RestaurantSystem.Api.Hubs;
using RestaurantSystem.Application.Contracts.Signals;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Services
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<OrderNotificationService> _logger;

        public OrderNotificationService(IHubContext<OrderHub> hubContext, ILogger<OrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        // 1. تنبيه بطلب جديد (يرسل للموظفين فقط في الـ StaffGroup)
        public async Task NotifyNewOrderAsync(OrderResponseDto orderResponse)
        {
            _logger.LogInformation("Broadcasting new order #{OrderNo} to StaffGroup.", orderResponse.OrderNumber);

            // نرسل للمجموعة الخاصة بالموظفين (StaffGroup) لكي لا يزعج التنبيه الزبائن
            await _hubContext.Clients.Group("StaffGroup").SendAsync("NewOrderPlaced", orderResponse);
        }

        // 2. تنبيه قسم معين (مثل شاشة المطبخ KDS)
        public async Task NotifyDepartmentAsync(string departmentId, object message)
        {
            _logger.LogInformation("Directing order items to Department: Dept_{DeptId}", departmentId);

            // نستخدم الـ Prefix "Dept_" لضمان التطابق مع ما برمجناه في الـ Hub
            await _hubContext.Clients.Group($"Dept_{departmentId}").SendAsync("NewItemsToPrepare", message);
        }

        // 3. تنبيه بتغيير حالة الطلب (وداعاً للـ ShortId الوهمي، أهلاً بالرقم الحقيقي)
        public async Task NotifyOrderStatusChangedAsync(Guid orderId, int orderNumber, string newStatus)
        {
            _logger.LogInformation("Status Update: Order #{OrderNo} is now {Status}", orderNumber, newStatus);

            // نرسل تحديثاً عاماً (للكاشير والمدير)
            await _hubContext.Clients.All.SendAsync("OrderStatusChanged", new
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                NewStatus = newStatus,
                DisplayMessage = $"الطلب رقم {orderNumber} أصبح الآن {newStatus}"
            });

            // تنبيه خاص لغرفة الزبون (Order Tracking Group) ليراها في تطبيقه فقط
            await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("MyOrderStatusUpdate", new
            {
                Status = newStatus,
                Message = $"وجبتك الآن في حالة: {newStatus}"
            });
        }

        // 4. 🆕 تنبيه خاص بتحديثات شركة التوصيل (سندي)
        // هذا الميثود هو الذي سيخبر الزبون "السائق اقترب منك"
        public async Task NotifyExternalDeliveryUpdateAsync(Guid orderId, int orderNumber, DeliveryPartnerStatus externalStatus, string message)
        {
            _logger.LogInformation("External Delivery Update for #{OrderNo}: {Status}", orderNumber, externalStatus);

            // نرسل التحديث حصرياً لغرفة الزبون المشترك بهذا الطلب
            await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("DeliveryTrackingUpdate", new
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                DeliveryStatus = externalStatus.ToString(),
                Description = message, // مثلاً: "السائق استلم الطلب وهو في الطريق"
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
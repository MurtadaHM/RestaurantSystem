using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Hubs;
using RestaurantSystem.Application.Contracts.Signals;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Services
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<OrderNotificationService> _logger;

        public OrderNotificationService(
            IHubContext<OrderHub> hubContext,
            ILogger<OrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyNewOrderAsync(OrderResponseDto orderResponse)
        {
            _logger.LogInformation(
                "Broadcasting new order #{OrderNo} to StaffGroup.",
                orderResponse.OrderNumber);

            await _hubContext.Clients
                .Group("StaffGroup")
                .SendAsync("NewOrderPlaced", orderResponse);
        }

        public async Task NotifyDepartmentAsync(string departmentId, object message)
        {
            _logger.LogInformation(
                "Directing order items to Department: Dept_{DeptId}",
                departmentId);

            await _hubContext.Clients
                .Group($"Dept_{departmentId}")
                .SendAsync("NewItemsToPrepare", message);
        }

        public async Task NotifyOrderStatusChangedAsync(
            Guid orderId,
            int orderNumber,
            string newStatus)
        {
            _logger.LogInformation(
                "Status Update: Order #{OrderNo} is now {Status}",
                orderNumber,
                newStatus);

            await _hubContext.Clients.All.SendAsync("OrderStatusChanged", new
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                NewStatus = newStatus,
                DisplayMessage = $"الطلب رقم {orderNumber} أصبح الآن {newStatus}"
            });

            await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("MyOrderStatusUpdate", new
            {
                Status = newStatus,
                Message = $"وجبتك الآن في حالة: {newStatus}"
            });
        }

        public async Task NotifyExternalDeliveryUpdateAsync(
            Guid orderId,
            int orderNumber,
            DeliveryPartnerStatus externalStatus,
            string message)
        {
            _logger.LogInformation(
                "External Delivery Update for #{OrderNo}: {Status}",
                orderNumber,
                externalStatus);

            await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("DeliveryTrackingUpdate", new
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                DeliveryStatus = externalStatus.ToString(),
                Description = message,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyTableStatusChangedAsync(
            Guid tableId,
            string tableNumber,
            string newStatus)
        {
            _logger.LogInformation(
                "Table Status Update: Table #{TableNumber} ({TableId}) is now {Status}",
                tableNumber,
                tableId,
                newStatus);

            await _hubContext.Clients.All.SendAsync("TableStatusChanged", new
            {
                tableId,
                tableNumber,
                newStatus,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyReservationStatusChangedAsync(
            Guid reservationId,
            string newStatus)
        {
            _logger.LogInformation(
                "Reservation Status Update: Reservation {ReservationId} is now {Status}",
                reservationId,
                newStatus);

            await _hubContext.Clients.All.SendAsync("ReservationStatusChanged", new
            {
                reservationId,
                newStatus,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
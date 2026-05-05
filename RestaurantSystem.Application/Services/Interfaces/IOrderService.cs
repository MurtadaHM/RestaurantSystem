using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.DTOs.PublicOrders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid id);
        Task<OrderResponseDto?> GetOrderByOrderNumberAsync(int orderNumber);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByTableIdAsync(Guid tableId);

        Task<OrderResponseDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequestDto request);
        Task<OrderResponseDto> UpdateOrderAsync(Guid id, CreateOrderRequestDto request);
        Task<bool> CancelOrderAsync(Guid id);
        Task<bool> DeleteOrderAsync(Guid id);

        Task<bool> PushOrderToExternalDeliveryAsync(Guid orderId);
        Task<OrderResponseDto> SyncExternalStatusAsync(Guid orderId);
        Task<OrderResponseDto> UpdateExternalStatusFromWebhookAsync(
            Guid externalOrderId,
            string newStatus,
            string? courierName,
            string? courierPhone,
            string? trackingUrl);

        Task<OrderResponseDto?> GetOrderByExternalIdAsync(Guid externalOrderId);

        Task<decimal> CalculateOrderTotalAsync(Guid orderId);
        Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();

        // NEW: Department-level progress
        Task<IEnumerable<OrderDepartmentProgressDto>> GetOrderDepartmentProgressAsync(Guid orderId);
        Task<OrderDepartmentProgressDto> UpdateOrderDepartmentStatusAsync(Guid orderId, UpdateOrderDepartmentStatusRequestDto request);

        // Push local delivery order to Sendy
        Task<OrderResponseDto> PushOrderToSendyAsync(Guid orderId);
    }
}
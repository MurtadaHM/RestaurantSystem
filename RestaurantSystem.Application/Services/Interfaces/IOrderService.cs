using System; // ✅ تأكد من إضافة هذا السطر للتعرف على Guid
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Application.DTOs.Orders;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IOrderService
    {
        // Create
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);

        // Read
        Task<OrderResponseDto> GetOrderByIdAsync(Guid id); // ✅ تم التغيير من int إلى Guid
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByTableIdAsync(Guid tableId); // ✅ تم التغيير من int إلى Guid

        // Update
        Task<OrderResponseDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequestDto request); // ✅ تم التغيير من int إلى Guid
        Task<OrderResponseDto> UpdateOrderAsync(Guid id, CreateOrderRequestDto request); // ✅ تم التغيير من int إلى Guid

        // Delete
        Task<bool> DeleteOrderAsync(Guid id); // ✅ تم التغيير من int إلى Guid

        // Business Logic
        Task<decimal> CalculateOrderTotalAsync(Guid orderId); // ✅ تم التغيير من int إلى Guid
        Task<bool> CancelOrderAsync(Guid id); // ✅ تم التغيير من int إلى Guid
        Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();
    }
}

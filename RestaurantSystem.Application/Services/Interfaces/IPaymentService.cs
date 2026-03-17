using RestaurantSystem.Application.DTOs.Payments;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        // Create
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request);

        // Read
        Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id);
        Task<PaymentResponseDto> GetPaymentByOrderIdAsync(Guid orderId);
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStatusAsync(PaymentStatus status);

        // Update
        Task<PaymentResponseDto> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequestDto request);

        // Business Logic
        Task<bool> RefundPaymentAsync(Guid id, string? notes);
        Task<bool> IsOrderPaidAsync(Guid orderId);
    }
}

using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Payments
{
    /// <summary>
    /// DTO لاستجابة عملية الدفع
    /// </summary>
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        /// <summary>المبلغ الكلي للطلب</summary>
        public decimal OrderTotalAmount { get; set; }

        public decimal Amount { get; set; }

        /// <summary>طريقة الدفع كـ String</summary>
        public string PaymentMethod { get; set; } = default!;

        /// <summary>حالة الدفع كـ String</summary>
        public string Status { get; set; } = default!;

        public string? TransactionReference { get; set; }

        public string? Notes { get; set; }

        /// <summary>تاريخ الدفع الفعلي — null لو لسه Pending</summary>
        public DateTime? PaymentDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

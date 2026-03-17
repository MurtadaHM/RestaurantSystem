using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Payments
{
    /// <summary>
    /// DTO لتحديث حالة الدفع
    /// </summary>
    public class UpdatePaymentStatusRequestDto
    {
        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        public PaymentStatus NewStatus { get; set; }

        /// <summary>
        /// رقم المرجع — مطلوب عند الـ Completed للدفع الإلكتروني
        /// </summary>
        public string? TransactionReference { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 500 حرف")]
        public string? Notes { get; set; }
    }
}

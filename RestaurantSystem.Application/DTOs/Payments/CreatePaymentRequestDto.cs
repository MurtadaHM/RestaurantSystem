using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Payments
{
    /// <summary>
    /// DTO لإنشاء عملية دفع جديدة
    /// </summary>
    public class CreatePaymentRequestDto
    {
        [Required(ErrorMessage = "معرّف الطلب مطلوب")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// رقم المرجع — مطلوب فقط للدفع الإلكتروني
        /// </summary>
        public string? TransactionReference { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 500 حرف")]
        public string? Notes { get; set; }
    }
}

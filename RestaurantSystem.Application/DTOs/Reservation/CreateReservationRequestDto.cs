using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Reservation
{
    public class CreateReservationRequestDto
    {
        [Required(ErrorMessage = "اسم الزبون مطلوب")]
        [MaxLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "رقم الهاتف يجب أن يكون 11 رقماً")]
        [RegularExpression(@"^\d+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام فقط")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ ووقت الحجز مطلوب")]
        public DateTime ReservationDate { get; set; }

        [Required(ErrorMessage = "عدد الأشخاص مطلوب")]
        [Range(1, 50, ErrorMessage = "عدد الضيوف يجب أن يكون بين 1 و 50")]
        public int GuestCount { get; set; }

        [Required(ErrorMessage = "يجب اختيار طاولة")]
        public Guid TableId { get; set; }

        public string? SpecialRequests { get; set; } // طلبات الزبون (مثلاً: ركن هادئ)

        // ✅ الإضافة اللي طلبتها: ملاحظات التحضير للموظفين
        public string? PreparationNotes { get; set; }
    }
}
using System;

namespace RestaurantSystem.Application.DTOs.Reservation
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public int GuestCount { get; set; }
        public string Status { get; set; } = string.Empty; // نص الحالة للعرض
        public string? SpecialRequests { get; set; }
        public string? PreparationNotes { get; set; }

        // بيانات الطاولة (نمرر الرقم والاسم بدلاً من الـ ID فقط لسهولة العرض)
        public Guid TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
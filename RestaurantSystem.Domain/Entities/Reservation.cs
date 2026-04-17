using System;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        // بيانات الزبون
        public string CustomerName { get; set; } = string.Empty; // ✅ Required
        public string CustomerPhone { get; set; } = string.Empty; // ✅ Required

        // تفاصيل الحجز
        public DateTime ReservationDate { get; set; } // موعد الحجز (يوم ووقت)
        public int GuestCount { get; set; }          // عدد الأشخاص
        public string? SpecialRequests { get; set; }  // ملاحظات (مثلاً: عيد ميلاد، طاولة خارجية)

        // الحالة الحالية للحجز
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // ──────────────────────────────────────────
        // العلاقات (Foreign Keys)
        // ──────────────────────────────────────────
        public Guid TableId { get; set; } // الطاولة المحجوزة

        // Navigation Properties
        public virtual Table? Table { get; set; }
    }
}
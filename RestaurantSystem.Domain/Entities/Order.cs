using System;
using System.Collections.Generic;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Order : BaseEntity
    {
        /// <summary>معرّف المستخدم</summary>
        public Guid UserId { get; set; }                    // ✅ Required

        /// <summary>معرّف الطاولة (اختياري)</summary>
        public Guid? TableId { get; set; }                  // ✅ nullable

        /// <summary>حالة الطلب</summary>
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>نوع الطلب</summary>
        public OrderType OrderType { get; set; }            // ✅ Required

        /// <summary>إجمالي السعر</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>رسوم التوصيل</summary>
        public decimal? DeliveryFee { get; set; }           // ✅ nullable

        /// <summary>ملاحظات خاصة</summary>
        public string? SpecialNotes { get; set; }           // ✅ FIX: nullable

        /// <summary>وقت الطلب المتوقع</summary>
        public DateTime? ExpectedReadyTime { get; set; }    // ✅ nullable

        /// <summary>وقت انتهاء الطلب الفعلي</summary>
        public DateTime? CompletedAt { get; set; }          // ✅ nullable

        // Navigation Properties
        public User? User { get; set; }
        public Table? Table { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}

using System;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        /// <summary>معرّف الطلب</summary>
        public Guid OrderId { get; set; }                          // ✅ Required

        /// <summary>طريقة الدفع</summary>
        public PaymentMethod PaymentMethod { get; set; }           // ✅ Required

        /// <summary>حالة الدفع</summary>
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>المبلغ المدفوع</summary>
        public decimal Amount { get; set; }                        // ✅ Required

        /// <summary>رقم المرجع (للدفع الإلكتروني)</summary>
        public string? TransactionReference { get; set; }          // ✅ FIX: nullable

        /// <summary>ملاحظات إضافية</summary>
        public string? Notes { get; set; }                         // ✅ FIX: nullable

        /// <summary>تاريخ الدفع الفعلي</summary>
        public DateTime? PaymentDate { get; set; }                 // ✅ nullable

        // Navigation Properties
        public Order? Order { get; set; }
    }
}

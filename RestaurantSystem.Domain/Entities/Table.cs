using System;
using System.Collections.Generic;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Table : BaseEntity
    {
        /// <summary>رقم الطاولة</summary>
        public string TableNumber { get; set; } = string.Empty; // ✅ Required

        /// <summary>عدد المقاعد</summary>
        public int Capacity { get; set; }                        // ✅ Required

        /// <summary>موقع الطاولة</summary>
        public string? Location { get; set; }                    // ✅ FIX: nullable

        /// <summary>حالة الطاولة</summary>
        public TableStatus Status { get; set; } = TableStatus.Available;

        /// <summary>ملاحظات إضافية</summary>
        public string? Notes { get; set; }                       // ✅ FIX: nullable

        // Navigation Properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        /// <summary>اسم المنتج</summary>
        public string Name { get; set; } = string.Empty;   // ✅ Required

        /// <summary>وصف المنتج</summary>
        public string? Description { get; set; }            // ✅ nullable

        /// <summary>سعر المنتج</summary>
        public decimal Price { get; set; }                  // ✅ Required

        /// <summary>صورة المنتج</summary>
        public string? ImageUrl { get; set; }               // ✅ nullable

        /// <summary>معرّف الفئة</summary>
        public Guid CategoryId { get; set; }                // ✅ Required

        /// <summary>هل المنتج متاح للطلب</summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>السعرات الحرارية (اختياري)</summary>
        public int? Calories { get; set; }                  // ✅ nullable

        /// <summary>المكونات الرئيسية</summary>
        public string? Ingredients { get; set; }            // ✅ nullable

        /// <summary>وقت التحضير بالدقائق</summary>
        public int PreparationTimeMinutes { get; set; } = 15;

        // Navigation Properties
        public Category? Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

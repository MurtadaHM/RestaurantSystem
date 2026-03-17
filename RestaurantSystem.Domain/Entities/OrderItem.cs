using System;

namespace RestaurantSystem.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        /// <summary>معرّف الطلب</summary>
        public Guid OrderId { get; set; }                          // ✅ Required

        /// <summary>معرّف المنتج</summary>
        public Guid MenuItemId { get; set; }                       // ✅ Required

        /// <summary>الكمية المطلوبة</summary>
        public int Quantity { get; set; }                          // ✅ Required

        /// <summary>سعر الوحدة عند الطلب</summary>
        public decimal Price { get; set; }                         // ✅ Required

        /// <summary>ملاحظات خاصة</summary>
        public string? SpecialInstructions { get; set; }           // ✅ FIX: nullable

        /// <summary>السعر الإجمالي للعنصر</summary>
        public decimal TotalPrice => Quantity * Price;              // ✅ Computed

        // Navigation Properties
        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}

using System;

namespace RestaurantSystem.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        /// <summary>معرّف الطلب</summary>
        public Guid OrderId { get; set; }

        /// <summary>معرّف المنتج</summary>
        public Guid MenuItemId { get; set; }

        /// <summary>القسم المسؤول عن تحضير هذا العنصر</summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>الكمية المطلوبة</summary>
        public int Quantity { get; set; }

        /// <summary>سعر الوحدة عند الطلب</summary>
        public decimal Price { get; set; }

        /// <summary>ملاحظات خاصة</summary>
        public string? SpecialInstructions { get; set; }

        /// <summary>السعر الإجمالي للعنصر</summary>
        public decimal TotalPrice => Quantity * Price;

        // Navigation Properties
        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
        public Department? Department { get; set; }
    }
}
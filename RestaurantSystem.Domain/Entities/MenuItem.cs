using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain.Entities
{
    /// <summary>
    /// صنف من أصناف المنيو (طبق، مشروب، إلخ)
    /// </summary>
    public class MenuItem : BaseEntity
    {
        /// <summary>اسم المنتج (مثلاً: كباب لحم، عصير ليمون)</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>وصف المنتج (المكونات أو طريقة التحضير)</summary>
        public string? Description { get; set; }

        /// <summary>سعر المنتج بالعملة المحلية</summary>
        public decimal Price { get; set; }

        /// <summary>رابط صورة المنتج</summary>
        public string? ImageUrl { get; set; }

        /// <summary>هل الصنف متاح حالياً للطلب؟</summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>السعرات الحرارية</summary>
        public int? Calories { get; set; }

        /// <summary>المكونات الرئيسية للحساسية أو المعلومات العامة</summary>
        public string? Ingredients { get; set; }

        /// <summary>الوقت المتوقع للتحضير (بالدقائق)</summary>
        public int PreparationTimeMinutes { get; set; } = 15;

        // ──────────────────────────────────────────
        // العلاقات (Foreign Keys)
        // ──────────────────────────────────────────

        /// <summary>معرّف القسم المسؤول عن التحضير (مطبخ، بارستا، أراكيل)</summary>
        public Guid DepartmentId { get; set; }

        /// <summary>معرّف الفئة التسويقية (مقبلات، مشروبات ساخنة، إلخ)</summary>
        public Guid CategoryId { get; set; }

        // ──────────────────────────────────────────
        // Navigation Properties
        // ──────────────────────────────────────────

        /// <summary>القسم التشغيلي المرتبط</summary>
        public virtual Department? Department { get; set; }

        /// <summary>الفئة المرتبطة</summary>
        public virtual Category? Category { get; set; }

        /// <summary>سجل الطلبات المرتبطة بهذا الصنف</summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // 🔥 التحديث الجديد لربط المخزن (الذكاء الاصطناعي للمطعم)
        /// <summary>المكونات الأولية (الوصفة) المرتبطة بهذا الصنف لغرض خصم المخزن</summary>
        public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
    }
}
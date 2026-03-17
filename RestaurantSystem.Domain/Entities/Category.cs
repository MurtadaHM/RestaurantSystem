using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain.Entities
{
    public class Category : BaseEntity
    {
        /// <summary>اسم الفئة</summary>
        public string Name { get; set; } = string.Empty;   // ✅ Required

        /// <summary>وصف الفئة</summary>
        public string? Description { get; set; }            // ✅ nullable

        /// <summary>صورة الفئة</summary>
        public string? ImageUrl { get; set; }               // ✅ FIX: nullable

        /// <summary>ترتيب الفئة في القائمة</summary>
        public int DisplayOrder { get; set; }

        // Navigation Properties
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}

namespace RestaurantSystem.Application.DTOs.Menu
{
    /// <summary>
    /// DTO لاستجابة المنتج شاملة بيانات الفئة والقسم التشغيلي
    /// </summary>
    public class MenuItemResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; } // ✅ تم التحويل لـ nullable

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // ✅ تم التحويل لـ nullable

        public string? Ingredients { get; set; } // ✅ تم التحويل لـ nullable

        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; }

        // بيانات الفئة (التصنيف التسويقي)
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;

        // ✅ إضافة بيانات القسم (مكان التحضير)
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = default!;

        public int OrderCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
namespace RestaurantSystem.Application.DTOs.Categories
{
    /// <summary>
    /// DTO لاستجابة الفئة شاملة بيانات القسم المرتبط
    /// </summary>
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        // ربط القسم (البيانات المطلوبة لعرضها في الجداول)
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = default!;

        public int MenuItemCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } // أضفناه لاكتمال البيانات
    }
}
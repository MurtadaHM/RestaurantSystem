namespace RestaurantSystem.Application.DTOs.Categories
{
    /// <summary>
    /// DTO لاستجابة الفئة
    /// </summary>
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;

        public int DisplayOrder { get; set; }

        public int MenuItemCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

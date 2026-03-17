namespace RestaurantSystem.Application.DTOs.Menu
{
    /// <summary>
    /// DTO لاستجابة المنتج
    /// </summary>
    public class MenuItemResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = default!;

        public string Ingredients { get; set; } = default!;

        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = default!;

        public int OrderCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

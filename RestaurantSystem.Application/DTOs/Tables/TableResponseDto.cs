namespace RestaurantSystem.Application.DTOs.Tables
{
    /// <summary>
    /// DTO لاستجابة الطاولة
    /// </summary>
    public class TableResponseDto
    {
        public Guid Id { get; set; }

        public string TableNumber { get; set; } = default!;

        public int Capacity { get; set; }

        public string Location { get; set; } = default!;

        public string Status { get; set; } = default!;

        public bool IsActive { get; set; }

        public int ActiveOrdersCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

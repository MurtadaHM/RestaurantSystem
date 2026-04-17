namespace RestaurantSystem.Application.DTOs.Integrations.Team6
{
    public class Team6ActiveOrdersResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Team6ActiveOrderDto> Data { get; set; } = new();
    }

    public class Team6ActiveOrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string RestaurantId { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;

        public Guid TableId { get; set; }
        public int TableNumber { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
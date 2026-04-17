namespace RestaurantSystem.Application.DTOs.Integrations.Team6
{
    public class Team6TableSessionResponseDto
    {
        public string RestaurantId { get; set; } = string.Empty;
        public Guid TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Zone { get; set; }
        public int? FloorNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsOrderingEnabled { get; set; }
    }
}
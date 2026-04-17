namespace RestaurantSystem.Application.DTOs.Integrations.Team6
{
    public class Team6OrderTrackingResponseDto
    {
        public string OrderId { get; set; } = string.Empty; // PartnerOrderId
        public string RestaurantId { get; set; } = string.Empty; // PartnerRestaurantId
        public Guid? TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string? UserId { get; set; } // PartnerUserId

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public bool IsActive { get; set; }
    }
}
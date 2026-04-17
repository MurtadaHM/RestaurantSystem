namespace RestaurantSystem.Application.DTOs.Integrations.Team6
{
    public class Team6UserOrderHistoryItemDto
    {
        public string PartnerOrderId { get; set; } = string.Empty;
        public string? PartnerUserId { get; set; }
        public string? PartnerRestaurantId { get; set; }

        public Guid InternalOrderId { get; set; }
        public int OrderNumber { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? ExternalDeliveryStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public string? Notes { get; set; }
    }
}
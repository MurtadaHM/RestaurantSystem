namespace RestaurantSystem.Application.DTOs.Integrations.Team6
{
    public class Team6OrderStatusResponseDto
    {
        public string PartnerOrderId { get; set; } = string.Empty;
        public Guid InternalOrderId { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ExternalDeliveryStatus { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string? Notes { get; set; }
    }
}
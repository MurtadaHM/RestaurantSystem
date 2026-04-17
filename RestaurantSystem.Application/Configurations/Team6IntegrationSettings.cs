namespace RestaurantSystem.Application.Configurations
{
    public class Team6IntegrationSettings
    {
        public bool Enabled { get; set; } = true;
        public string BaseUrl { get; set; } = string.Empty;
        public string ActiveOrdersPath { get; set; } = "/api/team10/orders/tracking";
        public string RestaurantId { get; set; } = string.Empty;
        public int PollingIntervalSeconds { get; set; } = 10;
        public Guid FallbackUserId { get; set; }
    }
}
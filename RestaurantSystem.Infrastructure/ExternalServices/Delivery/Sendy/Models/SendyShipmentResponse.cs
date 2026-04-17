using System.Text.Json.Serialization;

namespace RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy.Models
{
    /// <summary>
    /// البيانات الراجعة داخل data من Sendy بعد إنشاء order integration أو جلبه.
    /// </summary>
    public class SendyShipmentResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("trackingUrl")]
        public string? TrackingUrl { get; set; }

        [JsonPropertyName("courierName")]
        public string? CourierName { get; set; }

        [JsonPropertyName("courierPhone")]
        public string? CourierPhone { get; set; }
    }
}
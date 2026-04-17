using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSystem.Application.DTOs.External
{
    /// <summary>
    /// DTO لاستقبال Webhook updates من Sendy
    /// </summary>
    public class SendyWebhookPayload
    {
        /// <summary>
        /// External order id القادم من Sendy
        /// يطابق Order.ExternalOrderId عندنا
        /// </summary>
        [Required]
        [JsonPropertyName("order_id")]
        public Guid OrderId { get; set; }

        /// <summary>
        /// نوع الحدث
        /// مثال:
        /// shipment.status_changed
        /// courier.assigned
        /// </summary>
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("event_type")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// الحالة الجديدة القادمة من Sendy
        /// مثال:
        /// picked_up
        /// in_transit
        /// delivered
        /// </summary>
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("new_status")]
        public string NewStatus { get; set; } = string.Empty;

        /// <summary>
        /// وصف الحالة
        /// </summary>
        [MaxLength(500)]
        [JsonPropertyName("status_description")]
        public string? StatusDescription { get; set; }

        /// <summary>
        /// اسم السائق
        /// </summary>
        [MaxLength(150)]
        [JsonPropertyName("courier_name")]
        public string? CourierName { get; set; }

        /// <summary>
        /// رقم هاتف السائق
        /// </summary>
        [Phone]
        [JsonPropertyName("courier_phone")]
        public string? CourierPhone { get; set; }

        /// <summary>
        /// رابط التتبع
        /// </summary>
        [Url]
        [JsonPropertyName("tracking_url")]
        public string? TrackingUrl { get; set; }

        /// <summary>
        /// وقت إرسال الحدث من Sendy
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSystem.Application.DTOs.External
{
    /// <summary>
    /// DTO لاستقبال Webhook updates من Sendy
    /// يدعم شكل الـ payload المسطح (flat) وشكل الـ payload المتداخل (nested)
    /// </summary>
    public class SendyWebhookPayload
    {
        // ---------- Flat fields (optional now, controller will resolve precedence) ----------

        /// <summary>
        /// External order id القادم من Sendy (flat payload)
        /// يطابق Order.ExternalOrderId عندنا
        /// </summary>
        [JsonPropertyName("order_id")]
        public Guid? OrderId { get; set; }

        /// <summary>
        /// نوع الحدث (flat or nested)
        /// مثال:
        /// shipment.status_changed
        /// courier.assigned
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName("event_type")]
        public string? EventType { get; set; }

        /// <summary>
        /// الحالة الجديدة القادمة من Sendي (flat payload)
        /// مثال:
        /// picked_up
        /// in_transit
        /// delivered
        /// </summary>
        [MaxLength(100)]
        [JsonPropertyName("new_status")]
        public string? NewStatus { get; set; }

        /// <summary>
        /// وصف الحالة (flat payload)
        /// </summary>
        [MaxLength(500)]
        [JsonPropertyName("status_description")]
        public string? StatusDescription { get; set; }

        /// <summary>
        /// اسم السائق (flat payload)
        /// </summary>
        [MaxLength(150)]
        [JsonPropertyName("courier_name")]
        public string? CourierName { get; set; }

        /// <summary>
        /// رقم هاتف السائق (flat payload)
        /// </summary>
        [Phone]
        [JsonPropertyName("courier_phone")]
        public string? CourierPhone { get; set; }

        /// <summary>
        /// رابط التتبع (flat payload)
        /// </summary>
        [Url]
        [JsonPropertyName("tracking_url")]
        public string? TrackingUrl { get; set; }

        /// <summary>
        /// وقت إرسال الحدث من Sendy (flat payload)
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        // ---------- Nested fields (order object) ----------

        /// <summary>
        /// Occurred at time / top-level timestamp for nested payloads
        /// </summary>
        [JsonPropertyName("occurred_at")]
        public DateTime? OccurredAt { get; set; }

        /// <summary>
        /// Event id when present in nested payload
        /// </summary>
        [JsonPropertyName("event_id")]
        public Guid? EventId { get; set; }

        /// <summary>
        /// Nested order object used by Sendy's nested payload shape
        /// </summary>
        [JsonPropertyName("order")]
        public NestedOrder? Order { get; set; }

        public class NestedOrder
        {
            [JsonPropertyName("id")]
            public Guid? Id { get; set; }

            [JsonPropertyName("public_id")]
            public string? PublicId { get; set; }

            [JsonPropertyName("external_ref")]
            public string? ExternalRef { get; set; }

            [JsonPropertyName("fulfillment_type")]
            public string? FulfillmentType { get; set; }

            // provider status fields inside nested order
            [JsonPropertyName("status")]
            public string? Status { get; set; }

            [JsonPropertyName("from_status")]
            public string? FromStatus { get; set; }

            [JsonPropertyName("to_status")]
            public string? ToStatus { get; set; }

            [JsonPropertyName("changed_at")]
            public DateTime? ChangedAt { get; set; }

            [JsonPropertyName("notes")]
            public string? Notes { get; set; }
        }
    }
}
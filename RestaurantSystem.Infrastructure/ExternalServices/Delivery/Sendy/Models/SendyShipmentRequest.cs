using System.Text.Json.Serialization;

namespace RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy.Models
{
    /// <summary>
    /// الطلب المرسل إلى Sendy من جهة المتجر.
    /// ملاحظة: رغم اسم الكلاس القديم، هذا لم يعد Shipment بصيغته السابقة،
    /// بل صار مطابقاً لـ Store Integrations Order contract.
    /// </summary>
    public class SendyShipmentRequest
    {
        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = default!;

        [JsonPropertyName("customerPhone")]
        public string CustomerPhone { get; set; } = default!;

        [JsonPropertyName("customerAddress")]
        public string CustomerAddress { get; set; } = default!;

        [JsonPropertyName("deliveryLat")]
        public double DeliveryLat { get; set; }

        [JsonPropertyName("deliveryLng")]
        public double DeliveryLng { get; set; }

        [JsonPropertyName("orderValue")]
        public decimal OrderValue { get; set; }

        [JsonPropertyName("deliveryFee")]
        public decimal DeliveryFee { get; set; }

        [JsonPropertyName("externalRef")]
        public string ExternalRef { get; set; } = default!;

        [JsonPropertyName("fulfillmentType")]
        public string FulfillmentType { get; set; } = "from_to";

        [JsonPropertyName("deliveryMode")]
        public string DeliveryMode { get; set; } = "direct";

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; } = "cash";
    }
}
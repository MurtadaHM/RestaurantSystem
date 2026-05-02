namespace RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy
{
    /// <summary>
    /// إعدادات التكامل مع Sendy عبر Store Integrations API
    /// </summary>
    public class SendyClientSettings
    {
        /// <summary>
        /// الرابط الأساسي لـ Sendy API
        /// </summary>
        public string BaseUrl { get; set; } =
            "https://sendy-backend-production.up.railway.app/";

        /// <summary>
        /// مفتاح API الخاص بالمتجر
        /// يرسل داخل الهيدر: X-API-Key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// المسار الأساسي الخاص بواجهات التكامل للمتجر
        /// </summary>
        public string AdminRoute { get; set; } = "api/v1/store/integrations";

        /// <summary>
        /// السر الخاص بالـ Webhook القادم من Sendy
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;

        /// <summary>
        /// Phase 1: default province code to use when front-end did not provide address reference
        /// </summary>
        public string? DefaultAddressProvinceCode { get; set; }

        /// <summary>
        /// Phase 1: default area id to use when front-end did not provide address reference
        /// </summary>
        public Guid? DefaultAddressAreaId { get; set; }
    }
}
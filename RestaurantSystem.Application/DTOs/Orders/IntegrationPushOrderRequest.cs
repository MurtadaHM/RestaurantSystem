namespace RestaurantSystem.Application.DTOs.Orders
{
    /// <summary>
    /// DTO داخلي لإرسال الطلب من نظام المطعم إلى Sendy من جهة Store Integrations.
    /// </summary>
    public class IntegrationPushOrderRequest
    {
        /// <summary>
        /// رقم الطلب المحلي داخل نظام المطعم.
        /// مفيد للـ logs والتتبع الداخلي.
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// الاسم الكامل للزبون.
        /// </summary>
        public string CustomerName { get; set; } = default!;

        /// <summary>
        /// رقم هاتف الزبون.
        /// </summary>
        public string CustomerPhone { get; set; } = default!;

        /// <summary>
        /// عنوان الزبون النصي.
        /// </summary>
        public string CustomerAddress { get; set; } = default!;

        /// <summary>
        /// خط العرض لموقع التسليم.
        /// </summary>
        public double DeliveryLat { get; set; }

        /// <summary>
        /// خط الطول لموقع التسليم.
        /// </summary>
        public double DeliveryLng { get; set; }

        /// <summary>
        /// قيمة الطلب بدون رسوم التوصيل أو حسب ما يطلبه العقد الخارجي.
        /// </summary>
        public decimal OrderValue { get; set; }

        /// <summary>
        /// رسوم التوصيل.
        /// </summary>
        public decimal DeliveryFee { get; set; }

        /// <summary>
        /// المرجع الخارجي للطلب داخل نظام المطعم.
        /// الأفضل أن يكون ثابتاً وفريداً مثل:
        /// ORD-105 أو Local-{OrderId}
        /// </summary>
        public string ExternalRef { get; set; } = default!;

        /// <summary>
        /// نوع التنفيذ:
        /// from_to أو warehouse
        /// </summary>
        public string FulfillmentType { get; set; } = "from_to";

        /// <summary>
        /// نمط التوصيل كما يتطلبه Swagger الخاص بـ Sendy.
        /// </summary>
        public string DeliveryMode { get; set; } = "direct";

        /// <summary>
        /// طريقة الدفع:
        /// cash أو online
        /// </summary>
        public string PaymentMethod { get; set; } = "cash";

        /// <summary>
        /// NEW: address province code (address reference)
        /// </summary>
        public string? AddressProvinceCode { get; set; }

        /// <summary>
        /// NEW: address area id (address reference)
        /// </summary>
        public Guid? AddressAreaId { get; set; }
    }
}
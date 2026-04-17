namespace RestaurantSystem.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,      // قيد الانتظار
        Confirmed = 2,    // مؤكد
        Preparing = 3,    // قيد التحضير
        Ready = 4,        // جاهز (داخلياً)

        // 🆕 إضافات للربط مع Sendy
        ReadyForPickup = 5, // جاهز للاستلام (تم إبلاغ شركة التوصيل)
        Delivering = 6,     // قيد التوصيل (الطلب مع السائق حالياً)

        Completed = 7,    // مكتمل (تم التسليم بنجاح)
        Cancelled = 8,    // ملغي

        // 🆕 لدعم ميزة الإرجاع في نظام Sendy
        Returned = 9      // تم إرجاع الطلب
    }
}
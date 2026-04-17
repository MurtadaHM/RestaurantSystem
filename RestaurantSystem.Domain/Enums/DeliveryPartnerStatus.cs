namespace RestaurantSystem.Domain.Enums
{
    public enum DeliveryPartnerStatus
    {
        Idle = 0,               // لا يوجد طلب نشط حالياً
        SearchingForDriver = 1, // جاري البحث عن سائق متاح
        DriverAssigned = 2,     // تم قبول الطلب وتعيين سائق
        AtStore = 3,            // السائق وصل للمطعم وبانتظار الاستلام
        PickedUp = 4,           // السائق استلم الطلب وهو في الطريق
        ArrivedAtCustomer = 5,  // السائق وصل لموقع الزبون
        Delivered = 6,          // تم التسليم للزبون بنجاح
        Cancelled = 7,          // تم إلغاء المهمة من قبل شركة التوصيل
        Returned = 8,           // الطلب في طريق العودة للمطعم (مرتجع)
        Failed = 9              // فشل التوصيل لسبب تقني أو بشري
    }
}
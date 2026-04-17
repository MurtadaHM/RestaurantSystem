namespace RestaurantSystem.Domain.Enums
{
    public enum ReservationStatus
    {
        Pending = 1,    // حجز معلق (انتظار تأكيد) ⏳
        Confirmed = 2,  // حجز مؤكد ✅
        Completed = 3,  // الزبون حضر وانتهى الحجز 🍽️
        Cancelled = 4,  // تم إلغاء الحجز ❌
        NoShow = 5      // الزبون لم يحضر في الموعد 🚶‍♂️
    }
}
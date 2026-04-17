namespace RestaurantSystem.Domain.Enums
{
    public enum OrderType
    {
        DineIn = 1,       // داخل المطعم
        TakeAway = 2,     // استلام مباشر
        Delivery = 3      // توصيل (هذا النوع سيقوم بفتح قناة الاتصال مع Sendy)
    }
}
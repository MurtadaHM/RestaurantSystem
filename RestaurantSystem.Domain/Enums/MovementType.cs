
namespace RestaurantSystem.Domain.Enums
{
    public enum MovementType
    {
        Purchase = 1,    // شراء (إضافة للمخزن)
        Sale = 2,        // بيع (خصم تلقائي عند إتمام الطلب)
        Adjustment = 3,  // تعديل يدوي (عند الجرد)
        Waste = 4,       // إتلاف (مواد منتهية الصلاحية أو تالفة)
        Return = 5       // إرجاع للمجهز
    }
}

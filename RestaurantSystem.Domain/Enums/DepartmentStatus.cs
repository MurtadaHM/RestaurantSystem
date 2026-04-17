namespace RestaurantSystem.Domain.Enums
{
    public enum DepartmentStatus
    {
        Active = 1,      // القسم يعمل ويستقبل طلبات
        Inactive = 0,    // القسم متوقف حالياً
        Busy = 2         // القسم مزدحم جداً (لإظهار تنبيه للنادل)
    }
}
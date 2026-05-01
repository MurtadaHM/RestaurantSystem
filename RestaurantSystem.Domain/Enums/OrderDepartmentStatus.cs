namespace RestaurantSystem.Domain.Enums
{
    public enum OrderDepartmentStatus
    {
        Pending = 1,      // هذا القسم بعده ما باشر
        Preparing = 2,    // هذا القسم ديشتغل على عناصره
        Ready = 3         // هذا القسم خلص عناصره
    }
}
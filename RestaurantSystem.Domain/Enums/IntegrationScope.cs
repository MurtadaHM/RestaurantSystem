namespace RestaurantSystem.Domain.Enums
{
    public enum IntegrationScope
    {
        // صلاحيات الطلبات
        OrdersRead = 1,    // orders:read
        OrdersWrite = 2,   // orders:write

        // صلاحيات المنيو والمخزن
        CatalogRead = 3,   // catalog:read
        CatalogWrite = 4   // catalog:write
    }
}
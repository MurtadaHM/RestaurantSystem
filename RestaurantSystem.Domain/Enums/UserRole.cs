using System;

namespace RestaurantSystem.Domain.Enums
{
    public enum UserRole
    {
        Admin = 1,          // إدارة كاملة للنظام
        Manager = 2,        // إدارة تشغيلية
        Chef = 3,           // المطبخ
        Waiter = 4,         // النادل
        Cashier = 5,        // الكاشير
        DeliveryDriver = 6, // سائق التوصيل
        Customer = 7,       // العميل
        Barista = 8         // الباريستا
    }
}
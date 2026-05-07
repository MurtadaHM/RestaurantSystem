namespace RestaurantSystem.Domain.Enums
{
    public enum ActivityActionType
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        StatusChanged = 4,

        Login = 10,
        Logout = 11,

        OrderCreated = 20,
        OrderCancelled = 21,
        OrderStatusChanged = 22,
        DepartmentStatusChanged = 23,

        PublicOrderCreated = 30,

        PaymentCreated = 40,
        PaymentStatusChanged = 41,
        PaymentRefunded = 42,

        StockUpdated = 50,
        StockDeducted = 51,

        ReservationCreated = 60,
        ReservationUpdated = 61,
        ReservationDeleted = 62,
        ReservationStatusChanged = 63,

        TableCreated = 70,
        TableUpdated = 71,
        TableDeleted = 72,
        TableStatusChanged = 73,

        MenuItemCreated = 80,
        MenuItemUpdated = 81,
        MenuItemDeleted = 82,

        DepartmentCreated = 90,
        DepartmentUpdated = 91,
        DepartmentDeleted = 92,

        CategoryCreated = 100,
        CategoryUpdated = 101,
        CategoryDeleted = 102,

        UserCreated = 110,
        UserUpdated = 111,
        UserDeleted = 112,
        UserRoleChanged = 113,
        UserStatusChanged = 114,
        UserPasswordReset = 115
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Chef = 3,
        Waiter = 4,
        Cashier = 5,
        DeliveryDriver = 6,
        Customer = 7
    }
}

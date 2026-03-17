using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Enums
{
    public enum TableStatus
    {
        Available = 1,    // متاحة
        Occupied = 2,     // مشغولة
        Reserved = 3,     // محجوزة
        Maintenance = 4   // صيانة
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Enums
{
    public enum OrderType
    {
        DineIn = 1,       // داخل المطعم
        TakeAway = 2,     // استلام مباشر
        Delivery = 3      // توصيل
    }
}

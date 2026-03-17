using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,      // قيد الانتظار
        Confirmed = 2,    // مؤكد
        Preparing = 3,    // قيد التحضير
        Ready = 4,        // جاهز
        Delivering = 5,   // قيد التوصيل
        Completed = 6,    // مكتمل
        Cancelled = 7     // ملغي
    }
}

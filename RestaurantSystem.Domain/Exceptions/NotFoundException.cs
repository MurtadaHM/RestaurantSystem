using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // NotFoundException.cs
    // لما الشيء ما يتلاقى في قاعدة البيانات
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class NotFoundException : BaseException
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} with ID '{id}' was not found.", 404)
        { }

        public NotFoundException(string message)
            : base(message, 404)
        { }
    }

    // مثال على الاستخدام:
    // throw new NotFoundException("Order", orderId);
    // throw new NotFoundException("المطعم غير موجود");

}

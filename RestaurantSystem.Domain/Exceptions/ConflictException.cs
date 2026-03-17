using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ConflictException.cs
    // لما في تعارض - مثلاً إيميل مسجل
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, 409)
        { }
    }

    // مثال:
    // throw new ConflictException("هذا الإيميل مسجل مسبقاً");

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ForbiddenException.cs
    // مستخدم مسجل لكن ما عنده إذن
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "You do not have permission.")
            : base(message, 403)
        { }
    }

}

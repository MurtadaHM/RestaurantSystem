using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // UnauthorizedException.cs
    // لما المستخدم ما عنده صلاحية
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message, 401)
        { }
    }

}

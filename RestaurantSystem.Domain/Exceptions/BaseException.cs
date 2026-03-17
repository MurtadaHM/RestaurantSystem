using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{

    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // BaseException.cs
    // الأم اللي كل الأخطاء ترث منها
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; }

        protected BaseException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

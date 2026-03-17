using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain.Exceptions
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ValidationException.cs
    // لما البيانات المرسلة غلط
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class ValidationException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", 400)
        {
            Errors = errors;
        }
    }

}

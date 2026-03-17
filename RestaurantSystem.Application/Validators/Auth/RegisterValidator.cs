using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Validators.Auth
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // Auth/RegisterValidator.cs
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class RegisterValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("الاسم الأول مطلوب")
                .MaximumLength(50).WithMessage("الاسم الأول يجب أن لا يتجاوز 50 حرف");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("الاسم الأخير مطلوب")
                .MaximumLength(50).WithMessage("الاسم الأخير يجب أن لا يتجاوز 50 حرف");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("الإيميل مطلوب")
                .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(8).WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل")
                .Matches("[A-Z]").WithMessage("يجب أن تحتوي على حرف كبير")
                .Matches("[0-9]").WithMessage("يجب أن تحتوي على رقم");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("رقم الهاتف غير صحيح");
        }
    }

}

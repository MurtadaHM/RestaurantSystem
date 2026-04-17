using FluentValidation;
using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("الاسم الأول مطلوب")
                .MaximumLength(100).WithMessage("الاسم الأول يجب أن لا يتجاوز 100 حرف");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("الاسم الأخير مطلوب")
                .MaximumLength(100).WithMessage("الاسم الأخير يجب أن لا يتجاوز 100 حرف");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("الإيميل مطلوب")
                .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("رقم الهاتف غير صحيح");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(6).WithMessage("كلمة المرور يجب أن تكون 6 أحرف على الأقل")
                .Matches("[A-Z]").WithMessage("يجب أن تحتوي على حرف كبير")
                .Matches("[0-9]").WithMessage("يجب أن تحتوي على رقم");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("كلمة المرور وتأكيدها لا تتطابقان");
        }
    }
}
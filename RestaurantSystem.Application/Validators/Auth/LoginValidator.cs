using FluentValidation;
using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("الإيميل مطلوب")
                .EmailAddress().WithMessage("صيغة الإيميل غير صحيحة");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(6).WithMessage("كلمة المرور يجب أن تكون 6 أحرف على الأقل");
        }
    }
}
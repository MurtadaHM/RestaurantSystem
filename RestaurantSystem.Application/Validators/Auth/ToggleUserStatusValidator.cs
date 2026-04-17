using FluentValidation;
using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Validators.Auth
{
    public class ToggleUserStatusValidator : AbstractValidator<ToggleUserStatusRequestDto>
    {
        public ToggleUserStatusValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("حالة التفعيل مطلوبة");
        }
    }
}
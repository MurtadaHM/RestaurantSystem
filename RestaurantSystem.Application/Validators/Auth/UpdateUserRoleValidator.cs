using FluentValidation;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Validators.Auth
{
    public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleRequestDto>
    {
        public UpdateUserRoleValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("الدور غير صحيح")
                .Must(role => role != UserRole.Customer)
                .WithMessage("لا يمكن تعيين المستخدم كموظف بدور Customer");
        }
    }
}
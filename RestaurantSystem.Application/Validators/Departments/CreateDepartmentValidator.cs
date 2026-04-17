using FluentValidation;
using RestaurantSystem.Application.DTOs.Departments;

namespace RestaurantSystem.Application.Validators.Departments
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentRequestDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم القسم مطلوب")
                .MaximumLength(50).WithMessage("اسم القسم لا يمكن أن يتجاوز 50 حرفاً");
        }
    }
}
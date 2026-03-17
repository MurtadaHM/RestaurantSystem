using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RestaurantSystem.Application.DTOs.Menu;

namespace RestaurantSystem.Application.Validators.Menu
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // Menu/CreateMenuItemValidator.cs
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class CreateMenuItemValidator : AbstractValidator<CreateMenuItemRequestDto>
    {
        public CreateMenuItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم الطبق مطلوب")
                .MinimumLength(2).WithMessage("الاسم يجب أن يكون حرفين على الأقل")
                .MaximumLength(100).WithMessage("الاسم لا يتجاوز 100 حرف");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("السعر يجب أن يكون أكبر من صفر")
                .LessThanOrEqualTo(10000).WithMessage("السعر لا يمكن أن يتجاوز 10,000");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("التصنيف مطلوب");

            RuleFor(x => x.PreparationTimeMinutes)
                .GreaterThan(0).WithMessage("وقت التحضير يجب أن يكون أكبر من صفر")
                .LessThanOrEqualTo(180).WithMessage("وقت التحضير لا يتجاوز 180 دقيقة");
        }
    }

}

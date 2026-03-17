using FluentValidation;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Validators.Order
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("معرّف المستخدم مطلوب");

            // ✅ تم التغيير من Type إلى OrderType
            RuleFor(x => x.OrderType)
                .IsInEnum()
                .WithMessage("نوع الطلب غير صحيح");

            RuleFor(x => x.TableId)
                .NotNull()
                .WithMessage("رقم الطاولة مطلوب للطلبات داخل المطعم")
                .When(x => x.OrderType == OrderType.DineIn);

            RuleFor(x => x.DeliveryFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage("رسوم التوصيل لا يمكن أن تكون سالبة");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("الطلب يجب أن يحتوي على عنصر واحد على الأقل");

            // ✅ استدعاء الـ Validator الخاص بالعناصر بشكل صحيح
            RuleForEach(x => x.Items).SetValidator(new CreateOrderItemValidator());
        }
    }

    // ✅ كلاس جديد لفحص العناصر داخل القائمة
    public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemValidator()
        {
            RuleFor(x => x.MenuItemId).NotEmpty().WithMessage("معرف الوجبة مطلوب");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("الكمية يجب أن تكون 1 أو أكثر");
        }
    }
}
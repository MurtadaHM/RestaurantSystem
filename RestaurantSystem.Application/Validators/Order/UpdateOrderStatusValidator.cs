using System;
using FluentValidation;

namespace RestaurantSystem.Application.Validators.Order
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequestDto>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("معرف الطلب مطلوب");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("الحالة الجديدة يجب أن تكون من ضمن القيم المحددة");
        }
    }

    public class UpdateOrderStatusRequestDto
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Completed,
        Canceled
    }
}

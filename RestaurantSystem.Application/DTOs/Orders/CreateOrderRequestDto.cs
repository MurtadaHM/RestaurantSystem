using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    public class CreateOrderRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "معرّف المستخدم مطلوب")]
        public Guid UserId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? TableId { get; set; }

        [Required(ErrorMessage = "نوع الطلب مطلوب")]
        [EnumDataType(typeof(OrderType), ErrorMessage = "نوع الطلب غير صحيح")]
        public OrderType OrderType { get; set; }

        [MaxLength(1000, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 1000 حرف")]
        public string? SpecialNotes { get; set; }

        [Required(ErrorMessage = "يجب إضافة صنف واحد على الأقل للطلب")]
        [MinLength(1, ErrorMessage = "يجب إضافة صنف واحد على الأقل للطلب")]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [Range(0, 1000000, ErrorMessage = "رسوم التوصيل غير صالحة")]
        public decimal DeliveryFee { get; set; } = 0;

        // ──────────────────────────────────────────────────────────
        // Sendy Integration
        // ──────────────────────────────────────────────────────────

        [MaxLength(500, ErrorMessage = "العنوان طويل جداً")]
        public string? DeliveryAddress { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [MaxLength(50, ErrorMessage = "رقم الهاتف طويل جداً")]
        public string? CustomerPhoneNumber { get; set; }

        [Range(-90, 90, ErrorMessage = "خط العرض يجب أن يكون بين -90 و 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "خط الطول يجب أن يكون بين -180 و 180")]
        public double? Longitude { get; set; }

        // ──────────────────────────────────────────────────────────

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderType == OrderType.Delivery)
            {
                if (string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    yield return new ValidationResult(
                        "عنوان التوصيل مطلوب عندما يكون نوع الطلب Delivery",
                        new[] { nameof(DeliveryAddress) });
                }

                if (string.IsNullOrWhiteSpace(CustomerPhoneNumber))
                {
                    yield return new ValidationResult(
                        "رقم هاتف الزبون مطلوب عندما يكون نوع الطلب Delivery",
                        new[] { nameof(CustomerPhoneNumber) });
                }

                if (!Latitude.HasValue)
                {
                    yield return new ValidationResult(
                        "خط العرض مطلوب عندما يكون نوع الطلب Delivery",
                        new[] { nameof(Latitude) });
                }

                if (!Longitude.HasValue)
                {
                    yield return new ValidationResult(
                        "خط الطول مطلوب عندما يكون نوع الطلب Delivery",
                        new[] { nameof(Longitude) });
                }
            }

            if ((OrderType == OrderType.DineIn || OrderType == OrderType.TakeAway) && DeliveryFee > 0)
            {
                yield return new ValidationResult(
                    "رسوم التوصيل يجب أن تكون 0 إذا لم يكن الطلب Delivery",
                    new[] { nameof(DeliveryFee) });
            }
        }
    }

    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "معرّف المنتج مطلوب")]
        public Guid MenuItemId { get; set; }

        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
        public int Quantity { get; set; }

        [MaxLength(500, ErrorMessage = "التعليمات الخاصة لا يمكن أن تتجاوز 500 حرف")]
        public string? SpecialInstructions { get; set; }
    }
}
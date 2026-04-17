using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    /// <summary>
    /// DTO لتحديث حالة الطلب يدوياً من قبل الموظفين أو الإدارة
    /// </summary>
    public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "حالة الطلب غير موجودة في النظام")]
        public OrderStatus NewStatus { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 500 حرف")]
        public string? Notes { get; set; }
    }
}
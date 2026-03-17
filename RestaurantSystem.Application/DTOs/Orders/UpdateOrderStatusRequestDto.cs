using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    /// <summary>
    /// DTO لتحديث حالة الطلب
    /// </summary>
    public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "معرّف الطلب مطلوب")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        [RegularExpression("^(Pending|Processing|Ready|Completed|Cancelled)$",
            ErrorMessage = "حالة الطلب غير صحيحة")]
        public OrderStatus NewStatus { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 500 حرف")]
        public string Notes { get; set; } = default!;
    }
}

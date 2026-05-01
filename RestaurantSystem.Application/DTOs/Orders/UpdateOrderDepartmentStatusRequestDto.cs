using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    public class UpdateOrderDepartmentStatusRequestDto
    {
        [Required(ErrorMessage = "معرّف القسم مطلوب")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        [EnumDataType(typeof(OrderDepartmentStatus), ErrorMessage = "حالة القسم غير صحيحة")]
        public OrderDepartmentStatus NewStatus { get; set; }

        [MaxLength(500, ErrorMessage = "الملاحظات لا يمكن أن تتجاوز 500 حرف")]
        public string? Notes { get; set; }
    }
}
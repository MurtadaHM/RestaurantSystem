using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Auth
{
    public class ToggleUserStatusRequestDto
    {
        [Required(ErrorMessage = "حالة التفعيل مطلوبة")]
        public bool IsActive { get; set; }
    }
}
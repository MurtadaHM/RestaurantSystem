using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Auth
{
    public class UpdateUserRoleRequestDto
    {
        [Required(ErrorMessage = "الدور مطلوب")]
        public UserRole Role { get; set; }
    }
}
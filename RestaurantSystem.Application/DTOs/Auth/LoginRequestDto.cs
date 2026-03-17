using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Auth
{
    /// <summary>
    /// DTO لطلب تسجيل الدخول
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        public string Password { get; set; } = default!;
    }
}

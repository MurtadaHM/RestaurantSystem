using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Auth
{
    /// <summary>
    /// DTO لطلب التسجيل (إنشاء حساب جديد)
    /// </summary>
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم الأول لا يمكن أن يتجاوز 100 حرف")]
        public string FirstName { get; set; } = default!;

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم الأخير لا يمكن أن يتجاوز 100 حرف")]
        public string LastName { get; set; } = default!;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        public string PhoneNumber { get; set; } = default!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "كلمة المرور يجب أن تحتوي على أحرف كبيرة وصغيرة وأرقام ورموز")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها لا تتطابقان")]
        public string ConfirmPassword { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string City { get; set; } = default!;
    }
}

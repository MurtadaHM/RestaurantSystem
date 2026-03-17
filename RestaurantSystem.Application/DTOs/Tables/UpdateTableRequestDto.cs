using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Tables
{
    /// <summary>
    /// DTO لتحديث طاولة موجودة
    /// </summary>
    public class UpdateTableRequestDto
    {
        [Required(ErrorMessage = "معرّف الطاولة مطلوب")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "رقم الطاولة مطلوب")]
        [MaxLength(10, ErrorMessage = "رقم الطاولة لا يمكن أن يتجاوز 10 أحرف")]
        public string TableNumber { get; set; } = default!;

        [Required(ErrorMessage = "السعة مطلوبة")]
        [Range(1, 100, ErrorMessage = "السعة يجب أن تكون بين 1 و 100")]
        public int Capacity { get; set; }

        [MaxLength(100, ErrorMessage = "الموقع لا يمكن أن يتجاوز 100 حرف")]
        public string Location { get; set; } = default!;

        public bool IsActive { get; set; } = default!;
    }
}

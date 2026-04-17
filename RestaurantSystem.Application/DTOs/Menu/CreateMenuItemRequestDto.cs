using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Menu
{
    /// <summary>
    /// DTO لإنشاء منتج جديد في المنيو
    /// </summary>
    public class CreateMenuItemRequestDto
    {
        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المنتج لا يمكن أن يتجاوز 200 حرف")]
        public string Name { get; set; } = default!;

        [MaxLength(1000, ErrorMessage = "الوصف لا يمكن أن يتجاوز 1000 حرف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون قيمة موجبة")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "يجب اختيار القسم التشغيلي (مكان التحضير)")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الفئة التابع لها المنتج")]
        public Guid CategoryId { get; set; }

        // ✅ nullable لتجنب مشاكل الـ Validation أثناء التطوير
        public string? ImageUrl { get; set; }

        public string? Ingredients { get; set; }

        [Range(1, 120, ErrorMessage = "وقت التحضير يجب أن يكون بين دقيقة وساعتين")]
        public int PreparationTimeMinutes { get; set; } = 15;

        public bool IsAvailable { get; set; } = true;
    }
}
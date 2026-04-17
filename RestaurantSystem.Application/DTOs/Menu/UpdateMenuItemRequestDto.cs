using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Menu
{
    /// <summary>
    /// DTO لتحديث منتج موجود في المنيو
    /// </summary>
    public class UpdateMenuItemRequestDto
    {
        [Required(ErrorMessage = "معرّف المنتج مطلوب")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المنتج لا يمكن أن يتجاوز 200 حرف")]
        public string Name { get; set; } = default!;

        [MaxLength(1000, ErrorMessage = "وصف المنتج لا يمكن أن يتجاوز 1000 حرف")]
        public string? Description { get; set; } // ✅ جعلناه nullable للتوافق

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "يجب اختيار القسم التشغيلي")]
        public Guid DepartmentId { get; set; } // ✅ إضافة حقل القسم لضمان التوجيه الصحيح

        [Required(ErrorMessage = "معرّف الفئة مطلوب")]
        public Guid CategoryId { get; set; }

        // ✅ أزلنا [Url] لسهولة التطوير وجعلناه nullable
        public string? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "المكونات لا يمكن أن تتجاوز 500 حرف")]
        public string? Ingredients { get; set; }

        [Range(1, 120, ErrorMessage = "وقت التحضير يجب أن يكون بين 1 و 120 دقيقة")]
        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; }
    }
}
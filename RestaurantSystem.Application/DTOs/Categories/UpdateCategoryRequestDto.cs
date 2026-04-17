using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Categories
{
    /// <summary>
    /// DTO لتحديث فئة موجودة
    /// </summary>
    public class UpdateCategoryRequestDto
    {
        [Required(ErrorMessage = "معرّف الفئة مطلوب")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الفئة لا يمكن أن يتجاوز 100 حرف")]
        public string Name { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "وصف الفئة لا يمكن أن يتجاوز 500 حرف")]
        public string? Description { get; set; } // ✅ تم التحويل لـ nullable لكي يتطابق مع الـ Entity

        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ترتيب العرض يجب أن يكون أكبر من 0")]
        public int DisplayOrder { get; set; }

        [Required(ErrorMessage = "معرّف القسم مطلوب")]
        public Guid DepartmentId { get; set; }
    }
}
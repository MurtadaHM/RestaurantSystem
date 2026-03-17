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
        public string Description { get; set; } = default!;

        [Url(ErrorMessage = "رابط الصورة غير صحيح")]
        public string ImageUrl { get; set; } = default!;

        [Range(1, int.MaxValue, ErrorMessage = "ترتيب العرض يجب أن يكون أكبر من 0")]
        public int DisplayOrder { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Categories
{
    /// <summary>
    /// DTO لإنشاء فئة جديدة
    /// </summary>
    public class CreateCategoryRequestDto
    {
        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الفئة لا يمكن أن يتجاوز 100 حرف")]
        public string Name { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "وصف الفئة لا يمكن أن يتجاوز 500 حرف")]
        public string? Description { get; set; } // تم إزالة default! لكونه nullable

        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ترتيب العرض يجب أن يكون أكبر من 0")]
        public int DisplayOrder { get; set; } = 1;

        [Required(ErrorMessage = "معرّف القسم مطلوب")]
        public Guid DepartmentId { get; set; }
    }
}
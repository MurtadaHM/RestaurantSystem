using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Menu
{
    /// <summary>
    /// DTO لتحديث منتج موجود
    /// </summary>
    public class UpdateMenuItemRequestDto
    {
        [Required(ErrorMessage = "معرّف المنتج مطلوب")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المنتج لا يمكن أن يتجاوز 200 حرف")]
        public string Name { get; set; } = default!;

        [MaxLength(1000, ErrorMessage = "وصف المنتج لا يمكن أن يتجاوز 1000 حرف")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "معرّف الفئة مطلوب")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من 0")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "رابط الصورة غير صحيح")]
        public string ImageUrl { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "المكونات لا يمكن أن تتجاوز 500 حرف")]
        public string Ingredients { get; set; } = default!;

        [Range(1, 1000, ErrorMessage = "وقت التحضير يجب أن يكون بين 1 و 1000 دقيقة")]
        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; }
    }
}

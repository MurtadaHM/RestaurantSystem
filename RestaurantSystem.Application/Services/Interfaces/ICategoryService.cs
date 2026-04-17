using RestaurantSystem.Application.DTOs.Categories;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        // ✅ الإنشاء
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request);

        // ✅ القراءة
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        
        // 🆕 وظيفة إضافية: جلب الفئات التابعة لقسم معين (مثل فئات "البارستا" فقط)
        Task<IEnumerable<CategoryResponseDto>> GetCategoriesByDepartmentAsync(Guid departmentId);

        // ✅ التحديث
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request);

        // ✅ الحذف
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
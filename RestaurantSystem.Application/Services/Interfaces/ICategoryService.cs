using RestaurantSystem.Application.DTOs.Categories;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface ICategoryService  // ✅ public بدلاً من internal
    {
        // Create
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request);

        // Read
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();

        // Update
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request);

        // Delete
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}

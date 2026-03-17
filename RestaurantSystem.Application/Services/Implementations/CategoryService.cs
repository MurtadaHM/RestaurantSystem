using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Categories;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class CategoryService : ICategoryService  // ✅ public + implements interface
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // ──────────────────────────────────────────
        // Create
        // ──────────────────────────────────────────
        public async Task<CategoryResponseDto> CreateCategoryAsync(
            CreateCategoryRequestDto request)
        {
            // ✅ نتحقق من عدم تكرار الاسم
            var exists = await _categoryRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new Exception("اسم الفئة مستخدم بالفعل");

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        // ──────────────────────────────────────────
        // Read
        // ──────────────────────────────────────────
        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
        {
            // ✅ نستخدم دالة تجلب الفئة مع عدد المنتجات
            var category = await _categoryRepository.GetCategoryWithItemCountAsync(id);

            if (category == null)
                throw new Exception("الفئة غير موجودة");

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            // ✅ نجلب كل الفئات مع عدد منتجاتها
            var categories = await _categoryRepository.GetAllWithItemCountAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        // ──────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────
        public async Task<CategoryResponseDto> UpdateCategoryAsync(
            Guid id, UpdateCategoryRequestDto request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new Exception("الفئة غير موجودة");

            // ✅ نتحقق من تكرار الاسم فقط لو تغيّر
            if (!string.Equals(category.Name, request.Name,
                    StringComparison.OrdinalIgnoreCase))
            {
                var nameExists = await _categoryRepository.ExistsByNameAsync(request.Name);
                if (nameExists)
                    throw new Exception("اسم الفئة مستخدم بالفعل");
            }

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        // ──────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return false;

            // ✅ نمنع الحذف لو فيها منتجات
            var hasItems = await _categoryRepository.HasMenuItemsAsync(id);
            if (hasItems)
                throw new Exception("لا يمكن حذف الفئة لأنها تحتوي على منتجات");

            await _categoryRepository.DeleteAsync(id);
            return true;
        }
    }
}

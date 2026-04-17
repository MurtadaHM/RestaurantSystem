using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Categories;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class CategoryService : ICategoryService
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
        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request)
        {
            var exists = await _categoryRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new Exception("اسم الفئة مستخدم بالفعل");

            var category = _mapper.Map<Category>(request);

            await _categoryRepository.AddAsync(category);

            // لضمان استرجاع اسم القسم في الـ DTO بعد الإضافة
            return await GetCategoryByIdAsync(category.Id);
        }

        // ──────────────────────────────────────────
        // Read
        // ──────────────────────────────────────────
        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
        {
            // تأكد أن المستودع (Repository) يستخدم .Include(c => c.Department)
            var category = await _categoryRepository.GetCategoryWithItemCountAsync(id);

            if (category == null)
                throw new Exception("الفئة غير موجودة");

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            // 💡 ملاحظة مهندس: يجب أن تقوم دالة GetAllWithItemCountAsync 
            // بداخل الـ Repository بعمل .Include(c => c.Department)
            var categories = await _categoryRepository.GetAllWithItemCountAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        // 🆕 الوظيفة الجديدة: جلب الفئات التابعة لقسم معين
        public async Task<IEnumerable<CategoryResponseDto>> GetCategoriesByDepartmentAsync(Guid departmentId)
        {
            // نفترض وجود دالة في الـ Repository تفلتر حسب القسم مع Include
            var categories = await _categoryRepository.FindAsync(c => c.DepartmentId == departmentId && !c.IsDeleted);
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        // ──────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────
        public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new Exception("الفئة غير موجودة");

            if (!string.Equals(category.Name, request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nameExists = await _categoryRepository.ExistsByNameAsync(request.Name);
                if (nameExists)
                    throw new Exception("اسم الفئة مستخدم بالفعل");
            }

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            // نعود لجلبها بالكامل لضمان تحديث اسم القسم إذا تغير
            return await GetCategoryByIdAsync(category.Id);
        }

        // ──────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            var hasItems = await _categoryRepository.HasMenuItemsAsync(id);
            if (hasItems)
                throw new Exception("لا يمكن حذف الفئة لأنها تحتوي على منتجات");

            await _categoryRepository.DeleteAsync(id);
            return true;
        }
    }
}
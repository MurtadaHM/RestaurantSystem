using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    // ✅ نمرر context للأساس (base) فقط لتجنب تحذير الـ Capture
    public class CategoryRepository(ApplicationDbContext context)
        : Repository<Category>(context), ICategoryRepository
    {
        // ──────────────────────────────────────────
        // جلب فئة واحدة مع عدد منتجاتها
        // ──────────────────────────────────────────
        public async Task<Category?> GetCategoryWithItemCountAsync(Guid id)
        {
            // ✅ نستخدم _context (المورّث من الأب) بدلاً من context المعامل
            return await _context.Categories
                .Include(c => c.MenuItems
                    .Where(m => !m.IsDeleted)) // ✅ استثناء المحذوفين Soft Delete
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        // ──────────────────────────────────────────
        // جلب كل الفئات مع عدد منتجاتها مرتبة بـ DisplayOrder
        // ──────────────────────────────────────────
        public async Task<IEnumerable<Category>> GetAllWithItemCountAsync()
        {
            return await _context.Categories
                .Include(c => c.MenuItems
                    .Where(m => !m.IsDeleted)) // ✅ Soft Delete filter
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        // ──────────────────────────────────────────
        // التحقق من وجود فئة باسم معين
        // ──────────────────────────────────────────
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower()
                            && !c.IsDeleted); // ✅ نستثني المحذوفين
        }

        // ──────────────────────────────────────────
        // التحقق من وجود منتجات داخل الفئة
        // ──────────────────────────────────────────
        public async Task<bool> HasMenuItemsAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .AnyAsync(m => m.CategoryId == categoryId
                            && !m.IsDeleted); // ✅ Soft Delete filter
        }
    }
}
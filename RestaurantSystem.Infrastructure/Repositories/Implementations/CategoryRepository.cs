using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository(ApplicationDbContext context)
        : Repository<Category>(context), ICategoryRepository
    {
        // ──────────────────────────────────────────
        // جلب فئة واحدة مع القسم وعدد المنتجات
        // ──────────────────────────────────────────
        public async Task<Category?> GetCategoryWithItemCountAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Department) // ✅ السطر السحري: جلب بيانات القسم المرتبط
                .Include(c => c.MenuItems
                    .Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        // ──────────────────────────────────────────
        // جلب كل الفئات مع أقسامها ومنتجاتها
        // ──────────────────────────────────────────
        public async Task<IEnumerable<Category>> GetAllWithItemCountAsync()
        {
            return await _context.Categories
                .Include(c => c.Department) // ✅ السطر السحري: لكي يظهر الاسم في الجدول
                .Include(c => c.MenuItems
                    .Where(m => !m.IsDeleted))
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
                            && !c.IsDeleted);
        }

        // ──────────────────────────────────────────
        // التحقق من وجود منتجات داخل الفئة
        // ──────────────────────────────────────────
        public async Task<bool> HasMenuItemsAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .AnyAsync(m => m.CategoryId == categoryId
                            && !m.IsDeleted);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class MenuRepository : Repository<MenuItem>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        // 1. جلب طبق واحد مع تفاصيله الكاملة (تم إضافة OrderItems)
        public override async Task<MenuItem?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ مهم جداً لحساب عدد الطلبات
                .Include(m => m.MenuItemIngredients)
                    .ThenInclude(mi => mi.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        // 2. جلب كل المنيو (تم إضافة OrderItems لحل الخطأ 500)
        public override async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة هنا
                .Include(m => m.MenuItemIngredients)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        // 3. البحث والفلترة (تم تعميم إضافة OrderItems لكل الدوال لضمان الاستقرار)
        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة
                .Include(m => m.MenuItemIngredients)
                .Where(m => m.CategoryId == categoryId && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة
                .Include(m => m.MenuItemIngredients)
                .Where(m => m.DepartmentId == departmentId && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة
                .Include(m => m.MenuItemIngredients)
                .Where(m => m.IsAvailable && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<MenuItem>();

            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة
                .Include(m => m.MenuItemIngredients)
                .Where(m => m.Name.Contains(name) && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems) // ✅ تم الإضافة
                .Include(m => m.MenuItemIngredients)
                .Where(m => m.Price >= minPrice && m.Price <= maxPrice && !m.IsDeleted)
                .OrderBy(m => m.Price)
                .ToListAsync();
        }

        // 4. إحصائيات (الأكثر طلباً) - تبقى كما هي لأنها تحتوي على Include أصلاً
        public async Task<IEnumerable<MenuItem>> GetMostOrderedAsync(int topCount = 10)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Category)
                .Include(m => m.Department)
                .Include(m => m.OrderItems)
                .Include(m => m.MenuItemIngredients)
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.OrderItems.Count)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task UpdateAvailabilityAsync(Guid menuItemId, bool isAvailable)
        {
            var menuItem = await _dbSet.FindAsync(menuItemId);
            if (menuItem == null)
                throw new KeyNotFoundException($"MenuItem with id {menuItemId} not found");

            menuItem.IsAvailable = isAvailable;
            menuItem.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(menuItem);
        }

        // ✅ تنفيذ ميثود الحفظ لكي يتطابق الريبوزيتوري مع الواجهة (Interface)
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
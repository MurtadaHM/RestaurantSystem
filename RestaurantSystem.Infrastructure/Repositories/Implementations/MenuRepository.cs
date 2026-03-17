using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;
// ✅ 1. استدعاء الانترفيس من طبقة الـ Application (مهم جداً)
using RestaurantSystem.Application.Contracts.Repositories;
// ✅ 2. التأكد من مسار الـ DbContext (إذا استمر الخطأ، جرب تغيير Persistence إلى Data)
using RestaurantSystem.Infrastructure.Data; // ✅ التغيير هنا
namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class MenuRepository : Repository<MenuItem>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Where(m => m.CategoryId == categoryId && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            return await _dbSet
                .Where(m => m.IsAvailable && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<MenuItem>();

            return await _dbSet
                .Where(m => m.Name.Contains(name) && !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(m => m.Price >= minPrice && m.Price <= maxPrice && !m.IsDeleted)
                .OrderBy(m => m.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetMostOrderedAsync(int topCount = 10)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted)
                // تأكد أن OrderItems موجودة في الكلاس MenuItem كـ Navigation Property
                .Include(m => m.OrderItems)
                .OrderByDescending(m => m.OrderItems.Count)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task UpdateAvailabilityAsync(Guid menuItemId, bool isAvailable)
        {
            var menuItem = await GetByIdAsync(menuItemId);
            if (menuItem == null)
                throw new KeyNotFoundException($"MenuItem with id {menuItemId} not found");

            menuItem.IsAvailable = isAvailable;
            await UpdateAsync(menuItem);
        }
    }
}

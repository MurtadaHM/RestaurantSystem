using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<Ingredient?> GetByIdAsync(Guid id)
        {
            return await _context.Ingredients
                .Include(i => i.StockMovements)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task AddAsync(Ingredient ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
        }

        public void Update(Ingredient ingredient)
        {
            _context.Ingredients.Update(ingredient);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // 🔥 الوظائف الجديدة المضافة لحل أخطاء الـ Build وتفعيل الوصفات
        // ============================================================

        // 1. جلب مكونات طبق معين (من جدول الربط)
        public async Task<IEnumerable<MenuItemIngredient>> GetRecipeByMenuItemIdInternalAsync(Guid menuItemId)
        {
            return await _context.MenuItemIngredients
                .Where(mi => mi.MenuItemId == menuItemId)
                .ToListAsync();
        }

        // 2. إضافة عنصر جديد للوصفة
        public async Task AddRecipeItemAsync(MenuItemIngredient item)
        {
            await _context.MenuItemIngredients.AddAsync(item);
        }

        // 3. مسح مجموعة من العناصر (تُستخدم عند تحديث الوصفة لمسح القديم)
        public async Task RemoveRangeAsync(IEnumerable<MenuItemIngredient> entities)
        {
            _context.MenuItemIngredients.RemoveRange(entities);
            await Task.CompletedTask; // تنفيذ صوري ليتوافق مع الـ Async
        }

        // 4. جلب سجل حركات المادة مرتباً من الأحدث للأقدم
        public async Task<IEnumerable<StockMovement>> GetStockHistoryAsync(Guid ingredientId)
        {
            return await _context.StockMovements
                .Where(sm => sm.IngredientId == ingredientId)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();
        }

        // ملاحظة إضافية لضمان عمل GetWithRecipesAsync إذا كنت تستخدمه
        public async Task<Ingredient?> GetWithRecipesAsync(Guid id)
        {
            return await _context.Ingredients
                .Include(i => i.MenuItemIngredients)
                    .ThenInclude(mi => mi.MenuItem)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }
    }
}
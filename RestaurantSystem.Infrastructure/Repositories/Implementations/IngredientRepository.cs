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

        public async Task<IEnumerable<MenuItemIngredient>> GetRecipeByMenuItemIdInternalAsync(Guid menuItemId)
        {
            return await _context.MenuItemIngredients
                .IgnoreQueryFilters()
                .Where(mi => mi.MenuItemId == menuItemId && !mi.IsDeleted)
                .OrderBy(mi => mi.Id)
                .ToListAsync();
        }

        public async Task AddRecipeItemAsync(MenuItemIngredient item)
        {
            await _context.MenuItemIngredients.AddAsync(item);
        }

        public async Task RemoveRangeAsync(IEnumerable<MenuItemIngredient> entities)
        {
            _context.MenuItemIngredients.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task HardDeleteRecipeByMenuItemIdAsync(Guid menuItemId)
        {
            await _context.MenuItemIngredients
                .IgnoreQueryFilters()
                .Where(x => x.MenuItemId == menuItemId)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetStockHistoryAsync(Guid ingredientId)
        {
            return await _context.StockMovements
                .Where(sm => sm.IngredientId == ingredientId)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();
        }

        public async Task<Ingredient?> GetWithRecipesAsync(Guid id)
        {
            return await _context.Ingredients
                .Include(i => i.MenuItemIngredients)
                    .ThenInclude(mi => mi.MenuItem)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }
    }
}
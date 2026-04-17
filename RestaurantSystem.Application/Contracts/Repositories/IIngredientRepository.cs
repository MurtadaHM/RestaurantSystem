using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(Guid id);
        Task<Ingredient?> GetWithRecipesAsync(Guid id); // لجلب المادة مع وصفاتها
        Task AddAsync(Ingredient ingredient);
        void Update(Ingredient ingredient);
        Task SaveChangesAsync();

        // الوظائف الجديدة التي يطلبها الـ InventoryService
        Task<IEnumerable<MenuItemIngredient>> GetRecipeByMenuItemIdInternalAsync(Guid menuItemId);
        Task AddRecipeItemAsync(MenuItemIngredient item);
        Task RemoveRangeAsync(IEnumerable<MenuItemIngredient> entities);
        Task<IEnumerable<StockMovement>> GetStockHistoryAsync(Guid ingredientId);
    }
}

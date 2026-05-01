using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(Guid id);
        Task<Ingredient?> GetWithRecipesAsync(Guid id);
        Task AddAsync(Ingredient ingredient);
        void Update(Ingredient ingredient);
        Task SaveChangesAsync();

        Task<IEnumerable<MenuItemIngredient>> GetRecipeByMenuItemIdInternalAsync(Guid menuItemId);
        Task AddRecipeItemAsync(MenuItemIngredient item);
        Task RemoveRangeAsync(IEnumerable<MenuItemIngredient> entities);
        Task HardDeleteRecipeByMenuItemIdAsync(Guid menuItemId);
        Task<IEnumerable<StockMovement>> GetStockHistoryAsync(Guid ingredientId);
    }
}
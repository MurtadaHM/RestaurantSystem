using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IMenuRepository : IRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();
        Task<IEnumerable<MenuItem>> SearchByNameAsync(string name);
        Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<MenuItem>> GetMostOrderedAsync(int topCount = 10);
        Task UpdateAvailabilityAsync(Guid menuItemId, bool isAvailable);
    }
}

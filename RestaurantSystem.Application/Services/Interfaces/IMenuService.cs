using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Application.DTOs.Menu;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IMenuService
    {
        // Create
        Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemRequestDto request);

        // Read (تم تحويل int إلى Guid)
        Task<MenuItemResponseDto> GetMenuItemByIdAsync(Guid id);
        Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync();
        Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(Guid categoryId);

        // Update (تم تحويل int إلى Guid)
        Task<MenuItemResponseDto> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequestDto request);

        // Delete (تم تحويل int إلى Guid)
        Task<bool> DeleteMenuItemAsync(Guid id);

        // Search
        Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Menu;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public MenuService(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemRequestDto request)
        {
            var menuItem = _mapper.Map<MenuItem>(request);
            menuItem.CreatedAt = DateTime.UtcNow;

            await _menuRepository.AddAsync(menuItem);

            return _mapper.Map<MenuItemResponseDto>(menuItem);
        }

        public async Task<MenuItemResponseDto> GetMenuItemByIdAsync(Guid id)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);

            if (menuItem == null)
                throw new Exception("المنتج غير موجود");

            return _mapper.Map<MenuItemResponseDto>(menuItem);
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(Guid categoryId)
        {
            // ✅ تم تصحيح اسم الدالة لتطابق الموجودة في IMenuRepository
            var menuItems = await _menuRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        public async Task<MenuItemResponseDto> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequestDto request)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);

            if (menuItem == null)
                throw new Exception("المنتج غير موجود");

            _mapper.Map(request, menuItem);
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _menuRepository.UpdateAsync(menuItem);

            return _mapper.Map<MenuItemResponseDto>(menuItem);
        }

        public async Task<bool> DeleteMenuItemAsync(Guid id)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);

            if (menuItem == null)
                return false;

            await _menuRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm)
        {
            // ✅ تم تصحيح اسم الدالة لتطابق الموجودة في IMenuRepository
            var menuItems = await _menuRepository.SearchByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }
    }
}

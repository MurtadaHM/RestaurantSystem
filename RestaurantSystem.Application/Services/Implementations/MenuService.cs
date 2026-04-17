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

        // 1. إنشاء صنف جديد (تم إضافة الحفظ الفعلي)
        public async Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemRequestDto request)
        {
            var menuItem = _mapper.Map<MenuItem>(request);

            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.UpdatedAt = DateTime.UtcNow;
            menuItem.IsAvailable = true;

            await _menuRepository.AddAsync(menuItem);
            await _menuRepository.SaveChangesAsync(); // ✅ السطر الذهبي لضمان الحفظ

            return await GetMenuItemByIdAsync(menuItem.Id);
        }

        // 2. جلب صنف واحد بالـ ID
        public async Task<MenuItemResponseDto> GetMenuItemByIdAsync(Guid id)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);

            if (menuItem == null)
                throw new Exception("هذا الصنف غير موجود في المنيو");

            return _mapper.Map<MenuItemResponseDto>(menuItem);
        }

        // 3. جلب كل المنيو (سيعمل بسلام بعد تعديل الـ Include في الـ Repo)
        public async Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        // 4. جلب منتجات فئة معينة
        public async Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(Guid categoryId)
        {
            var menuItems = await _menuRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        // 5. جلب منتجات قسم معين (مطبخ، بارستا..)
        public async Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByDepartmentAsync(Guid departmentId)
        {
            var menuItems = await _menuRepository.GetByDepartmentAsync(departmentId);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        // 6. تعديل صنف موجود
        public async Task<MenuItemResponseDto> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequestDto request)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);

            if (menuItem == null)
                throw new Exception("الصنف المطلوب تعديله غير موجود");

            _mapper.Map(request, menuItem);
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _menuRepository.UpdateAsync(menuItem);
            await _menuRepository.SaveChangesAsync(); // ✅ تحديث قاعدة البيانات

            return await GetMenuItemByIdAsync(menuItem.Id);
        }

        // 7. حذف صنف (Soft Delete)
        public async Task<bool> DeleteMenuItemAsync(Guid id)
        {
            var menuItem = await _menuRepository.GetByIdAsync(id);
            if (menuItem == null) return false;

            await _menuRepository.DeleteAsync(id);
            await _menuRepository.SaveChangesAsync(); // ✅ تأكيد الحذف
            return true;
        }

        // 8. البحث الذكي بالاسم
        public async Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm)
        {
            var menuItems = await _menuRepository.SearchByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(menuItems);
        }

        // ==========================================
        // 🔥 تنفيذ الوظائف الجديدة المضافة للـ Interface
        // ==========================================

        public async Task<IEnumerable<MenuItemResponseDto>> GetAvailableMenuItemsAsync()
        {
            var items = await _menuRepository.GetAvailableItemsAsync();
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(items);
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetMostOrderedMenuItemsAsync(int topCount = 10)
        {
            var items = await _menuRepository.GetMostOrderedAsync(topCount);
            return _mapper.Map<IEnumerable<MenuItemResponseDto>>(items);
        }

        public async Task<bool> ToggleMenuItemAvailabilityAsync(Guid id, bool isAvailable)
        {
            try
            {
                await _menuRepository.UpdateAvailabilityAsync(id, isAvailable);
                await _menuRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
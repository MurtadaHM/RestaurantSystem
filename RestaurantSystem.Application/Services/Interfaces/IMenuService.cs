using RestaurantSystem.Application.DTOs.Menu;

namespace RestaurantSystem.Application.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمات إدارة أصناف المنيو (المحرك البرمجي لقائمة الطعام)
    /// </summary>
    public interface IMenuService
    {
        // ──────────────────────────────────────────
        // 1. العمليات الأساسية (CRUD)
        // ──────────────────────────────────────────
        Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemRequestDto request);
        Task<MenuItemResponseDto> GetMenuItemByIdAsync(Guid id);
        Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync();
        Task<MenuItemResponseDto> UpdateMenuItemAsync(Guid id, UpdateMenuItemRequestDto request);
        Task<bool> DeleteMenuItemAsync(Guid id);

        // ──────────────────────────────────────────
        // 2. الفلترة والبحث الذكي
        // ──────────────────────────────────────────
        Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByDepartmentAsync(Guid departmentId);
        Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm);

        // ──────────────────────────────────────────
        // 3. وظائف متقدمة (Management & Analytics) 🔥
        // ──────────────────────────────────────────

        // جلب الأصناف المتوفرة فقط (تستخدم في منيو الزبائن)
        Task<IEnumerable<MenuItemResponseDto>> GetAvailableMenuItemsAsync();

        // جلب الأصناف الأكثر مبيعاً (للإحصائيات)
        Task<IEnumerable<MenuItemResponseDto>> GetMostOrderedMenuItemsAsync(int topCount = 10);

        // تحديث حالة التوفر (متوفر/منفذ) بسرعة من شاشة المطبخ
        Task<bool> ToggleMenuItemAvailabilityAsync(Guid id, bool isAvailable);
    }
}
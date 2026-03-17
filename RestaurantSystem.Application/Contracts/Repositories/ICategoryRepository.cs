using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Interfaces; // ✅ تأكد من استدعاء مسار IRepository

namespace RestaurantSystem.Application.Contracts.Repositories
{
    // ✅ يجب إضافة : IRepository<Category> لكي ترث كل الدوال الأساسية
    public interface ICategoryRepository : IRepository<Category>
    {
        // الدوال الخاصة بالفئات فقط (التي لا توجد في الـ Generic Repository)
        Task<Category?> GetCategoryWithItemCountAsync(Guid id);
        Task<IEnumerable<Category>> GetAllWithItemCountAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> HasMenuItemsAsync(Guid categoryId);
    }
}
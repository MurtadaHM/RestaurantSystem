using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    // ✅ الآن يورث من IRepository الخاص بك
    public interface IDepartmentRepository : IRepository<Department>
    {
        // عمليات خاصة بالأقسام فقط
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
        Task<Department?> GetWithMenuItemsAsync(Guid id);
    }
}
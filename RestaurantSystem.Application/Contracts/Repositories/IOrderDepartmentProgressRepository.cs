using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IOrderDepartmentProgressRepository
    {
        Task<IEnumerable<OrderDepartmentProgress>> GetByOrderIdAsync(Guid orderId);
        Task<OrderDepartmentProgress?> GetByOrderAndDepartmentAsync(Guid orderId, Guid departmentId);
        Task AddAsync(OrderDepartmentProgress entity);
        Task UpdateAsync(OrderDepartmentProgress entity);
        Task SaveChangesAsync();
    }
}
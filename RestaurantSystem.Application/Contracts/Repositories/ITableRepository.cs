using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface ITableRepository : IRepository<Table>
    {
        Task<Table?> GetByTableNumberAsync(string tableNumber);
        Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status);
        Task<IEnumerable<Table>> GetAvailableTablesAsync();
        Task<IEnumerable<Table>> GetByLocationAsync(string location);
        Task<IEnumerable<Table>> GetByCapacityAsync(int capacity);

        // ✅ Guid بدل int
        Task<Table?> GetByIdWithOrdersAsync(Guid id);

        Task<bool> ExistsByTableNumberAsync(string tableNumber);

        // ✅ Guid بدل int
        Task UpdateStatusAsync(Guid tableId, TableStatus status);
    }
}

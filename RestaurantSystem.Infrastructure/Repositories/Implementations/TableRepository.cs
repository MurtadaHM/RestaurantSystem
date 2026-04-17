using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class TableRepository : Repository<Table>, ITableRepository
    {
        public TableRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Table?> GetByTableNumberAsync(string tableNumber)
        {
            var normalizedTableNumber = tableNumber.Trim();

            return await _dbSet
                .FirstOrDefaultAsync(t => t.TableNumber == normalizedTableNumber);
        }

        public async Task<Table?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim();

            return await _dbSet
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Code == normalizedCode);
        }

        public async Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Table>> GetAvailableTablesAsync()
        {
            return await _dbSet
                .Where(t =>
                    t.Status == TableStatus.Available &&
                    t.IsActive &&
                    t.IsOrderingEnabled)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Table>> GetByLocationAsync(string location)
        {
            var normalizedLocation = location.Trim().ToLower();

            return await _dbSet
                .Where(t =>
                    t.Location != null &&
                    t.Location.ToLower() == normalizedLocation)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Table>> GetByCapacityAsync(int capacity)
        {
            return await _dbSet
                .Where(t => t.Capacity >= capacity)
                .OrderBy(t => t.Capacity)
                .ToListAsync();
        }

        public async Task<Table?> GetByIdWithOrdersAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsByTableNumberAsync(string tableNumber)
        {
            var normalizedTableNumber = tableNumber.Trim();

            return await _dbSet
                .AnyAsync(t => t.TableNumber == normalizedTableNumber);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            var normalizedCode = code.Trim();

            return await _dbSet
                .AnyAsync(t => t.Code == normalizedCode);
        }

        public async Task UpdateStatusAsync(Guid tableId, TableStatus status)
        {
            var table = await _dbSet.FindAsync(tableId);

            if (table is not null)
            {
                table.Status = status;
                _dbSet.Update(table);
            }
        }
    }
}
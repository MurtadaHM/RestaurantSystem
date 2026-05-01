using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class OrderDepartmentProgressRepository : IOrderDepartmentProgressRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<OrderDepartmentProgress> _dbSet;

        public OrderDepartmentProgressRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<OrderDepartmentProgress>();
        }

        public async Task<IEnumerable<OrderDepartmentProgress>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .Include(x => x.Department)
                .Where(x => x.OrderId == orderId && !x.IsDeleted)
                .OrderBy(x => x.Department!.Name)
                .ToListAsync();
        }

        public async Task<OrderDepartmentProgress?> GetByOrderAndDepartmentAsync(Guid orderId, Guid departmentId)
        {
            return await _dbSet
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x =>
                    x.OrderId == orderId &&
                    x.DepartmentId == departmentId &&
                    !x.IsDeleted);
        }

        public async Task AddAsync(OrderDepartmentProgress entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(OrderDepartmentProgress entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
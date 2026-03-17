using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    // ✅ استخدام الـ Primary Constructor وتمريره للأب (base) مباشرة
    public class OrderRepository(ApplicationDbContext context)
        : Repository<Order>(context), IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(o => o.UserId == userId && !o.IsDeleted)
                               .OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await _dbSet.Where(o => o.Status == OrderStatus.Pending && !o.IsDeleted)
                               .OrderBy(o => o.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet.Where(o => o.Status == status && !o.IsDeleted)
                               .OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByTableIdAsync(Guid tableId)
        {
            return await _dbSet.Where(o => o.TableId == tableId && !o.IsDeleted)
                               .OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && !o.IsDeleted)
                               .OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate
                                      && o.Status == OrderStatus.Completed && !o.IsDeleted)
                               .SumAsync(o => o.TotalAmount);
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await UpdateAsync(order);
            }
        }

        // ✅ تحويل نوع الإرجاع إلى Order? (Nullable) لحل تحذير الـ Null reference return
        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _dbSet.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                               .Include(o => o.User).Include(o => o.Table).Include(o => o.Payment)
                               .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                               .Include(o => o.User).Include(o => o.Table).Include(o => o.Payment)
                               .Where(o => !o.IsDeleted).OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task DeleteOrderWithItemsAsync(Guid orderId)
        {
            var order = await GetOrderWithDetailsAsync(orderId);
            if (order != null)
            {
                // ✅ استخدام check للتأكد من أن OrderItems ليست null قبل التحديث
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.UtcNow;
                    }
                }

                order.IsDeleted = true;
                order.DeletedAt = DateTime.UtcNow;
                await UpdateAsync(order);
            }
        }
    }
}
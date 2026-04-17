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
    public class OrderRepository(ApplicationDbContext context)
        : Repository<Order>(context), IOrderRepository
    {
        // 1️⃣ جلب الطلبات المعلقة (KDS)
        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == OrderStatus.Pending && !o.IsDeleted)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();
        }

        // 2️⃣ البحث عن طريق المعرف الخارجي (مهم جداً لربط سندي والـ Webhooks)
        public async Task<Order?> GetByExternalIdAsync(Guid externalId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.ExternalOrderId == externalId && !o.IsDeleted);
        }

        // 3️⃣ تحديث حالة التوصيل الخارجي وبيانات السائق
        public async Task UpdateExternalDeliveryInfoAsync(Guid orderId, DeliveryPartnerStatus status, string? courierName, string? courierPhone)
        {
            var order = await _dbSet.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.ExternalDeliveryStatus = status;
                order.LastExternalSyncDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(courierName)) order.CourierName = courierName;
                if (!string.IsNullOrEmpty(courierPhone)) order.CourierPhoneNumber = courierPhone;

                // إذا وصل السائق للعميل أو سلم الطلب، نحدث الحالة الداخلية تلقائياً
                if (status == DeliveryPartnerStatus.Delivered)
                {
                    order.Status = OrderStatus.Completed;
                    order.CompletedAt = DateTime.UtcNow;
                }

                await UpdateAsync(order);
            }
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Status == status && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.User)
                .Include(o => o.Table)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        // 🔥 الميثود الجديدة لخدمة نظام المخزن (Deep Eager Loading)
        public async Task<Order?> GetOrderWithDetailsForInventoryAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        // ⬇️ هنا نضع علامة ! بعد mi و mii
                        .ThenInclude(mi => mi!.MenuItemIngredients)
                            .ThenInclude(mii => mii!.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _dbSet.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;

                if (newStatus == OrderStatus.Completed)
                    order.CompletedAt = DateTime.UtcNow;

                await UpdateAsync(order);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByTableIdAsync(Guid tableId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.TableId == tableId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate
                      && o.Status == OrderStatus.Completed && !o.IsDeleted)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.User)
                .Include(o => o.Table)
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteOrderWithItemsAsync(Guid orderId)
        {
            var order = await GetOrderWithDetailsAsync(orderId);
            if (order != null)
            {
                var now = DateTime.UtcNow;
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = now;
                    }
                }
                order.IsDeleted = true;
                order.DeletedAt = now;
                await UpdateAsync(order);
            }
        }

        public async Task<Order?> GetByPartnerOrderIdAsync(string partnerOrderId, string partnerSource)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o =>
                    o.PartnerOrderId == partnerOrderId &&
                    o.PartnerSource == partnerSource &&
                    !o.IsDeleted);
        }
        public async Task<IEnumerable<Order>> GetActivePartnerOrdersAsync(string partnerSource)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Table)
                .Where(o =>
                    !o.IsDeleted &&
                    o.PartnerSource == partnerSource &&
                    o.Status != OrderStatus.Cancelled)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByPartnerUserIdAsync(string partnerUserId, string partnerRestaurantId, string partnerSource)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Table)
                .Include(o => o.Payment)
                .Where(o =>
                    o.PartnerUserId == partnerUserId &&
                    o.PartnerRestaurantId == partnerRestaurantId &&
                    o.PartnerSource == partnerSource &&
                    !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
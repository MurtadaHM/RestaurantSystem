using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data; 
namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class PaymentRepository(ApplicationDbContext context) : Repository<Payment>(context), IPaymentRepository
    {

        // ──────────────────────────────────────────
        // جلب الدفع الخاص بطلب معين مع تفاصيل الطلب
        // ──────────────────────────────────────────
        public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        {
            return await context.Payments
                .Include(p => p.Order)         // ✅ نجلب الطلب للـ TotalAmount
                .FirstOrDefaultAsync(p => p.OrderId == orderId
                                       && !p.IsDeleted);
        }

        // ──────────────────────────────────────────
        // جلب كل المدفوعات بحالة معينة مرتبة بالتاريخ
        // ──────────────────────────────────────────
        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            // ✅ الـ Status مخزن كـ string في PostgreSQL
            var statusString = status.ToString();

            return await context.Payments
                .Include(p => p.Order)
                .Where(p => p.Status.ToString() == statusString
                         && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ──────────────────────────────────────────
        // التحقق من وجود دفع مكتمل للطلب
        // ──────────────────────────────────────────
        public async Task<bool> IsOrderPaidAsync(Guid orderId)
        {
            var completedStatus = PaymentStatus.Completed.ToString();

            return await context.Payments
                .AnyAsync(p => p.OrderId == orderId
                            && p.Status.ToString() == completedStatus
                            && !p.IsDeleted);
        }
    }
}

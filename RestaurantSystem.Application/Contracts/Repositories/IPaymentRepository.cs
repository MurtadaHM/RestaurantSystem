using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Interfaces; // ✅ أضفنا هذا السطر للوصول للـ Repository الأساسي

namespace RestaurantSystem.Application.Contracts.Repositories
{
    // ✅ قمنا بتغيير IGenericRepository إلى IRepository لتطابق ما بنيناه سابقاً
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>
        /// جلب الدفع الخاص بطلب معين
        /// </summary>
        Task<Payment?> GetByOrderIdAsync(Guid orderId);

        /// <summary>
        /// جلب كل المدفوعات بحالة معينة
        /// </summary>
        Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);

        /// <summary>
        /// التحقق من وجود دفع مكتمل للطلب
        /// </summary>
        Task<bool> IsOrderPaidAsync(Guid orderId);
    }
}
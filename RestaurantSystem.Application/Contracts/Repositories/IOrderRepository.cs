using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    /// <summary>
    /// واجهة إدارة عمليات الطلبات في قاعدة البيانات
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        // ──────────────────────────────────────────────────────────
        // 🏗️ عمليات القراءة (Read)
        // ──────────────────────────────────────────────────────────

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);

        Task<IEnumerable<Order>> GetPendingOrdersAsync();

        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

        Task<IEnumerable<Order>> GetOrdersByTableIdAsync(Guid tableId);

        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<Order?> GetOrderWithDetailsAsync(Guid orderId);

        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();



        /// <summary>
        /// 🔍 البحث عن الطلب باستخدام المعرف الخاص بشركة التوصيل (Sendy)
        /// ضروري جداً لمعالجة إشعارات الـ Webhook
        /// </summary>
        Task<Order?> GetByExternalIdAsync(Guid externalId);

        /// <summary>
        /// 🔥 جلب الطلب مع كافة تفاصيل المكونات والوصفات لغرض خصم المخزن
        /// </summary>
        Task<Order?> GetOrderWithDetailsForInventoryAsync(Guid orderId);

        // ──────────────────────────────────────────────────────────
        // 📈 عمليات الإحصائيات والبحث المعمق
        // ──────────────────────────────────────────────────────────

        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);

        // ──────────────────────────────────────────────────────────
        // 🔄 عمليات التحديث والحذف
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// تحديث الحالة الداخلية للطلب (مثل Pending -> Confirmed)
        /// </summary>
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        /// <summary>
        /// 🆕 تحديث بيانات التوصيل الخارجي (حالة سندي، بيانات السائق، وتاريخ المزامنة)
        /// </summary>
        Task UpdateExternalDeliveryInfoAsync(Guid orderId, DeliveryPartnerStatus status, string? courierName, string? courierPhone);

        Task DeleteOrderWithItemsAsync(Guid orderId);

        Task<Order?> GetByPartnerOrderIdAsync(string partnerOrderId, string partnerSource);
        Task<IEnumerable<Order>> GetByPartnerUserIdAsync(string partnerUserId, string partnerRestaurantId, string partnerSource);
        Task<IEnumerable<Order>> GetActivePartnerOrdersAsync(string partnerSource);
    }
}
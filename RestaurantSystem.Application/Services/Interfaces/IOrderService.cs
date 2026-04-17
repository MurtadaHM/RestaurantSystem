using RestaurantSystem.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantSystem.Application.Services.Interfaces
{
    /// <summary>
    /// واجهة برمجية لإدارة دورة حياة الطلب بالكامل.
    /// تربط بين إدارة الصالة، المطعم، المخزن، وشركة التوصيل (Sendy).
    /// </summary>
    public interface IOrderService
    {
        // ──────────────────────────────────────────────────────────
        // 🏗️ عمليات الإنشاء والقراءة (Create & Read)
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// إنشاء طلب جديد وتوليد رقم طلب (OrderNumber) بسيط وتنبيه النظام.
        /// </summary>
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);

        /// <summary>
        /// جلب تفاصيل الطلب باستخدام الـ GUID.
        /// </summary>
        Task<OrderResponseDto> GetOrderByIdAsync(Guid id);

        /// <summary>
        /// جلب تفاصيل الطلب باستخدام رقم الطلب البسيط (OrderNumber).
        /// </summary>
        Task<OrderResponseDto?> GetOrderByOrderNumberAsync(int orderNumber);

        /// <summary>
        /// جلب كافة الطلبات.
        /// </summary>
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();

        /// <summary>
        /// جلب تاريخ طلبات مستخدم معين.
        /// </summary>
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId);

        /// <summary>
        /// جلب الطلبات النشطة لطاولة معينة.
        /// </summary>
        Task<IEnumerable<OrderResponseDto>> GetOrdersByTableIdAsync(Guid tableId);

        // ──────────────────────────────────────────────────────────
        // 🔄 عمليات التحديث والإلغاء (Update & Delete)
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// تحديث الحالة الداخلية للطلب.
        /// عند الوصول إلى Confirmed لطلب Delivery يتم إرسال الطلب تلقائياً إلى Sendy إذا لم يكن مرسلاً مسبقاً.
        /// </summary>
        Task<OrderResponseDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequestDto request);

        /// <summary>
        /// تحديث بيانات الطلب القابلة للتعديل مثل العنوان والملاحظات.
        /// </summary>
        Task<OrderResponseDto> UpdateOrderAsync(Guid id, CreateOrderRequestDto request);

        /// <summary>
        /// إلغاء الطلب محلياً، مع محاولة إلغائه خارجياً في Sendy إذا كان مرسلاً مسبقاً.
        /// </summary>
        Task<bool> CancelOrderAsync(Guid id);

        /// <summary>
        /// حذف الطلب (Soft Delete).
        /// </summary>
        Task<bool> DeleteOrderAsync(Guid id);

        // ──────────────────────────────────────────────────────────
        // 🚚 وظائف التكامل مع Sendy
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// إرسال الطلب يدوياً إلى Sendy.
        /// </summary>
        Task<bool> PushOrderToExternalDeliveryAsync(Guid orderId);

        /// <summary>
        /// مزامنة حالة الطلب من Sendy عبر polling.
        /// </summary>
        Task<OrderResponseDto> SyncExternalStatusAsync(Guid orderId);

        /// <summary>
        /// تحديث حالة الطلب مباشرة من webhook القادم من Sendy.
        /// </summary>
        Task<OrderResponseDto> UpdateExternalStatusFromWebhookAsync(
            Guid externalOrderId,
            string newStatus,
            string? courierName,
            string? courierPhone,
            string? trackingUrl);

        /// <summary>
        /// البحث عن الطلب المحلي باستخدام ExternalOrderId.
        /// </summary>
        Task<OrderResponseDto?> GetOrderByExternalIdAsync(Guid externalOrderId);

        // ──────────────────────────────────────────────────────────
        // 🧠 منطق الأعمال المتقدم
        // ──────────────────────────────────────────────────────────

        /// <summary>
        /// حساب إجمالي الطلب.
        /// </summary>
        Task<decimal> CalculateOrderTotalAsync(Guid orderId);

        /// <summary>
        /// جلب الطلبات المعلقة.
        /// </summary>
        Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();
    }
}
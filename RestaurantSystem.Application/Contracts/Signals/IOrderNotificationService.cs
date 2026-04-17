using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Signals
{
    /// <summary>
    /// واجهة إدارة التنبيهات الفورية (SignalR)
    /// مسؤولة عن إبقاء جميع أطراف النظام (مطبخ، كاشير، زبون) على اطلاع بلحظة بلحظة
    /// </summary>
    public interface IOrderNotificationService
    {
        /// <summary>
        /// إرسال تنبيه بوجود طلب جديد (للكاشير وشاشة المطبخ)
        /// استخدمنا OrderResponseDto لضمان وصول الـ OrderNumber والبيانات كاملة
        /// </summary>
        Task NotifyNewOrderAsync(OrderResponseDto orderResponse);

        /// <summary>
        /// إرسال تنبيه لقسم معين (مثل إرسال طلبات المشويات للمطبخ فقط)
        /// </summary>
        Task NotifyDepartmentAsync(string departmentId, object message);

        /// <summary>
        /// تنبيه بتغيير حالة الطلب الداخلية (مثلاً: جاري التحضير، جاهز)
        /// </summary>
        /// <param name="orderId">المعرف الفريد</param>
        /// <param name="orderNumber">الرقم البسيط ليظهر في التنبيه (مثلاً: طلب #105 جاهز)</param>
        /// <param name="newStatus">الحالة الجديدة</param>
        Task NotifyOrderStatusChangedAsync(Guid orderId, int orderNumber, string newStatus);

        /// <summary>
        /// 🆕 تنبيه خاص بتحديثات شركة التوصيل (سندي)
        /// يُستخدم لإبلاغ الزبون بمكان السائق أو عندما يتم استلام الطلب منه
        /// </summary>
        Task NotifyExternalDeliveryUpdateAsync(Guid orderId, int orderNumber, DeliveryPartnerStatus externalStatus, string message);
    }
}
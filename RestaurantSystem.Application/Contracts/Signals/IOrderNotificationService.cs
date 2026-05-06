using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Signals
{
    /// <summary>
    /// واجهة إدارة التنبيهات الفورية (SignalR)
    /// مسؤولة عن إبقاء جميع أطراف النظام على اطلاع بلحظة بلحظة
    /// </summary>
    public interface IOrderNotificationService
    {
        /// <summary>
        /// إرسال تنبيه بوجود طلب جديد
        /// </summary>
        Task NotifyNewOrderAsync(OrderResponseDto orderResponse);

        /// <summary>
        /// إرسال تنبيه لقسم معين
        /// </summary>
        Task NotifyDepartmentAsync(string departmentId, object message);

        /// <summary>
        /// تنبيه بتغيير حالة الطلب الداخلية
        /// </summary>
        Task NotifyOrderStatusChangedAsync(Guid orderId, int orderNumber, string newStatus);

        /// <summary>
        /// تنبيه خاص بتحديثات شركة التوصيل
        /// </summary>
        Task NotifyExternalDeliveryUpdateAsync(
            Guid orderId,
            int orderNumber,
            DeliveryPartnerStatus externalStatus,
            string message);

        /// <summary>
        /// تنبيه بتغيير حالة الطاولة
        /// </summary>
        Task NotifyTableStatusChangedAsync(
            Guid tableId,
            string tableNumber,
            string newStatus);

        /// <summary>
        /// تنبيه بتغيير حالة الحجز
        /// </summary>
        Task NotifyReservationStatusChangedAsync(
            Guid reservationId,
            string newStatus);
    }
}
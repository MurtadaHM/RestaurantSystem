using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        // جلب حجوزات اليوم
        Task<IEnumerable<Reservation>> GetTodayReservationsAsync();

        // جلب الحجوزات لطاولة معينة في تاريخ محدد
        Task<IEnumerable<Reservation>> GetReservationsByTableAndDateAsync(Guid tableId, DateTime date);

        // البحث عن حجز برقم الهاتف
        Task<Reservation?> GetByCustomerPhoneAsync(string phone);

        // تحديث حالة الحجز بسرعة
        Task UpdateStatusAsync(Guid reservationId, ReservationStatus status);
    }
}
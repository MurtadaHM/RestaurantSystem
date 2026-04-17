using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
        }

        // 1. جلب حجوزات اليوم باستخدام مقارنة النطاق (لحل مشكلة PostgreSQL date_trunc)
        public async Task<IEnumerable<Reservation>> GetTodayReservationsAsync()
        {
            // ✅ التغيير لـ Now ليتطابق مع ساعة جهازك وقاعدة البيانات
            var todayStart = DateTime.Now.Date;
            var tomorrowStart = todayStart.AddDays(1);

            return await _dbSet
                .Include(r => r.Table)
                .Where(r => r.ReservationDate >= todayStart &&
                            r.ReservationDate < tomorrowStart &&
                            !r.IsDeleted)
                .OrderBy(r => r.ReservationDate)
                .ToListAsync();
        }

        // 2. فحص التضارب لنفس الطاولة واليوم
        public async Task<IEnumerable<Reservation>> GetReservationsByTableAndDateAsync(Guid tableId, DateTime date)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            return await _dbSet
                .Where(r => r.TableId == tableId &&
                            r.ReservationDate >= dayStart &&
                            r.ReservationDate < dayEnd &&
                            !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByCustomerPhoneAsync(string phone)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.CustomerPhone == phone && !r.IsDeleted);
        }

        // 3. تحديث حالة الحجز مع الحفظ الفوري
        public async Task UpdateStatusAsync(Guid reservationId, ReservationStatus status)
        {
            var reservation = await _dbSet.Include(r => r.Table).FirstOrDefaultAsync(x => x.Id == reservationId);
            if (reservation != null)
            {
                reservation.Status = status;
                reservation.UpdatedAt = DateTime.Now;

                // 🔥 المبدأ الذكي: إذا تم التأكيد وكان الموعد قريباً (أقل من 60 دقيقة)
                // نغير حالة الطاولة فوراً دون انتظار الـ Worker
                if (status == ReservationStatus.Confirmed)
                {
                    var diff = reservation.ReservationDate - DateTime.Now;
                    if (diff.TotalMinutes <= 60 && diff.TotalMinutes >= -15)
                    {
                        if (reservation.Table != null && reservation.Table.Status == TableStatus.Available)
                        {
                            reservation.Table.Status = TableStatus.Reserved;
                        }
                    }
                }
                // إذا تم الإلغاء، نرجع الطاولة متاحة فوراً
                else if (status == ReservationStatus.Cancelled)
                {
                    if (reservation.Table != null && reservation.Table.Status == TableStatus.Reserved)
                    {
                        reservation.Table.Status = TableStatus.Available;
                    }
                }

                _context.Entry(reservation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
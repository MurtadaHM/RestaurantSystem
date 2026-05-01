using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Reservation;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository reservationRepository,
            ITableRepository tableRepository,
            IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        // ──────────────────────────────────────────
        // 1. إنشاء حجز جديد (مع فحص التضارب)
        // ──────────────────────────────────────────
        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationRequestDto request)
        {
            // أ. التأكد من أن التاريخ ليس في الماضي
            // Use local restaurant time comparison (DateTime.Now) to avoid UTC/local mismatch
            if (request.ReservationDate < DateTime.Now)
                throw new Exception("لا يمكن الحجز في تاريخ قديم!");

            // ب. التأكد من وجود الطاولة وسعتها
            var table = await _tableRepository.GetByIdAsync(request.TableId);
            if (table == null) throw new Exception("الطاولة المختارة غير موجودة.");

            if (table.Capacity < request.GuestCount)
                throw new Exception($"هذه الطاولة تكفي لـ {table.Capacity} أشخاص فقط، وعدد ضيوفك {request.GuestCount}.");

            // ج. خوارزمية فحص تضارب المواعيد (Conflict Check)
            // نجلب كل حجوزات هذه الطاولة في نفس اليوم
            var existingReservations = await _reservationRepository
                .GetReservationsByTableAndDateAsync(request.TableId, request.ReservationDate);

            foreach (var existing in existingReservations)
            {
                // نحسب الفرق بالوقت بين الحجز new والحجوزات الموجودة
                var timeDiff = (request.ReservationDate - existing.ReservationDate).Duration();

                // إذا كان الفرق أقل من ساعتين (120 دقيقة)، نرفض الحجز
                if (timeDiff.TotalMinutes < 120)
                {
                    throw new Exception($"عذراً، الطاولة محجوزة في هذا الوقت (حجز آخر في {existing.ReservationDate:HH:mm}). يرجى اختيار وقت مختلـف.");
                }
            }

            // د. حفظ الحجز
            var reservation = _mapper.Map<Reservation>(request);
            reservation.Status = ReservationStatus.Pending; // الحالة الأولية

            var result = await _reservationRepository.AddAsync(reservation);

            // نرجع البيانات كاملة مع معلومات الطاولة
            return _mapper.Map<ReservationResponseDto>(result);
        }

        // ──────────────────────────────────────────
        // 2. جلب حجوزات اليوم
        // ──────────────────────────────────────────
        public async Task<IEnumerable<ReservationResponseDto>> GetTodayReservationsAsync()
        {
            var reservations = await _reservationRepository.GetTodayReservationsAsync();
            return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);
        }

        public async Task<ReservationResponseDto> GetByIdAsync(Guid id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) throw new Exception("الحجز غير موجود.");
            return _mapper.Map<ReservationResponseDto>(reservation);
        }

        public async Task UpdateStatusAsync(Guid id, ReservationStatus status)
        {
            await _reservationRepository.UpdateStatusAsync(id, status);
        }

        public async Task DeleteReservationAsync(Guid id)
        {
            await _reservationRepository.DeleteAsync(id);
        }
    }
}
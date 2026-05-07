using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.ActivityLogs;
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
        private readonly IActivityLogService _activityLogService;
        private readonly IMapper _mapper;

        public ReservationService(
            IReservationRepository reservationRepository,
            ITableRepository tableRepository,
            IActivityLogService activityLogService,
            IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _tableRepository = tableRepository;
            _activityLogService = activityLogService;
            _mapper = mapper;
        }

        // ──────────────────────────────────────────
        // 1. إنشاء حجز جديد مع فحص التضارب
        // ──────────────────────────────────────────
        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationRequestDto request)
        {
            // Use local restaurant time comparison to avoid UTC/local mismatch
            if (request.ReservationDate < DateTime.Now)
                throw new Exception("لا يمكن الحجز في تاريخ قديم!");

            var table = await _tableRepository.GetByIdAsync(request.TableId);
            if (table == null)
                throw new Exception("الطاولة المختارة غير موجودة.");

            if (table.Capacity < request.GuestCount)
                throw new Exception($"هذه الطاولة تكفي لـ {table.Capacity} أشخاص فقط، وعدد ضيوفك {request.GuestCount}.");

            var existingReservations = await _reservationRepository
                .GetReservationsByTableAndDateAsync(request.TableId, request.ReservationDate);

            foreach (var existing in existingReservations)
            {
                var timeDiff = (request.ReservationDate - existing.ReservationDate).Duration();

                if (timeDiff.TotalMinutes < 120)
                {
                    throw new Exception(
                        $"عذراً، الطاولة محجوزة في هذا الوقت (حجز آخر في {existing.ReservationDate:HH:mm}). يرجى اختيار وقت مختلف.");
                }
            }

            var reservation = _mapper.Map<Reservation>(request);
            reservation.Status = ReservationStatus.Pending;

            var result = await _reservationRepository.AddAsync(reservation);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.ReservationCreated,
                Module = "Reservations",
                EntityName = nameof(Reservation),
                EntityId = result.Id,
                Description =
                    $"Created reservation for customer '{result.CustomerName}' on table {table.TableNumber} at {result.ReservationDate:g}.",
                NewValue =
                    $"CustomerName={result.CustomerName}; Phone={result.CustomerPhone}; Table={table.TableNumber}; Guests={result.GuestCount}; Status={result.Status}; ReservationDate={result.ReservationDate:g}"
            });

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

            if (reservation == null)
                throw new Exception("الحجز غير موجود.");

            return _mapper.Map<ReservationResponseDto>(reservation);
        }

        public async Task UpdateStatusAsync(Guid id, ReservationStatus status)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);

            if (reservation == null)
                throw new Exception("الحجز غير موجود.");

            var oldStatus = reservation.Status;

            await _reservationRepository.UpdateStatusAsync(id, status);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.ReservationStatusChanged,
                Module = "Reservations",
                EntityName = nameof(Reservation),
                EntityId = reservation.Id,
                Description =
                    $"Changed reservation status for customer '{reservation.CustomerName}' from {oldStatus} to {status}.",
                OldValue = oldStatus.ToString(),
                NewValue = status.ToString()
            });
        }

        public async Task DeleteReservationAsync(Guid id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);

            if (reservation == null)
                throw new Exception("الحجز غير موجود.");

            await _reservationRepository.DeleteAsync(id);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.ReservationDeleted,
                Module = "Reservations",
                EntityName = nameof(Reservation),
                EntityId = reservation.Id,
                Description =
                    $"Deleted reservation for customer '{reservation.CustomerName}' scheduled at {reservation.ReservationDate:g}.",
                OldValue =
                    $"CustomerName={reservation.CustomerName}; Phone={reservation.CustomerPhone}; Guests={reservation.GuestCount}; Status={reservation.Status}; ReservationDate={reservation.ReservationDate:g}"
            });
        }

        private async Task SafeLogActivityAsync(CreateActivityLogDto dto)
        {
            try
            {
                await _activityLogService.LogAsync(dto);
            }
            catch
            {
                // Activity logging should never break the main reservation operation.
            }
        }
    }
}
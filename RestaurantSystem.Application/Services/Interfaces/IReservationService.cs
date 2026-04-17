using RestaurantSystem.Application.DTOs.Reservation;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationResponseDto>> GetAllReservationsAsync();
        Task<IEnumerable<ReservationResponseDto>> GetTodayReservationsAsync();
        Task<ReservationResponseDto> GetByIdAsync(Guid id);
        Task<ReservationResponseDto> CreateReservationAsync(CreateReservationRequestDto request);
        Task UpdateStatusAsync(Guid id, ReservationStatus status);
        Task DeleteReservationAsync(Guid id);
    }
}
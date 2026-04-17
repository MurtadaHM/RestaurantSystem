using RestaurantSystem.Application.DTOs.Integrations.Team6;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface ITeam6TrackingService
    {
        Task<Team6OrderTrackingResponseDto> GetOrderTrackingAsync(string partnerOrderId);
    }
}
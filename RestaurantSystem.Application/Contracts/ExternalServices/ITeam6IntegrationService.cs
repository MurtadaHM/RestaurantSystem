using RestaurantSystem.Application.DTOs.Integrations.Team6;

namespace RestaurantSystem.Application.Contracts.ExternalServices
{
    public interface ITeam6IntegrationService
    {
        Task<Team6ActiveOrdersResponseDto?> GetActiveOrdersAsync(CancellationToken cancellationToken = default);
    }
}
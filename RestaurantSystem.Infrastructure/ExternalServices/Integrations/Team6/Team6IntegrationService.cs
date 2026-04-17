using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.DTOs.Integrations.Team6;
using RestaurantSystem.Application.Configurations;

namespace RestaurantSystem.Infrastructure.ExternalServices.Integrations.Team6
{
    public class Team6IntegrationService : ITeam6IntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly Team6IntegrationSettings _settings;
        private readonly ILogger<Team6IntegrationService> _logger;

        public Team6IntegrationService(
            HttpClient httpClient,
            IOptions<Team6IntegrationSettings> settings,
            ILogger<Team6IntegrationService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<Team6ActiveOrdersResponseDto?> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("Team6 integration is disabled.");
                return null;
            }

            try
            {
                var response = await _httpClient.GetAsync(_settings.ActiveOrdersPath, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Team6 active orders request failed. StatusCode: {StatusCode}", response.StatusCode);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<Team6ActiveOrdersResponseDto>(
                    cancellationToken: cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch active orders from Team6.");
                return null;
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Configurations;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Infrastructure.BackgroundServices
{
    public class Team6OrderSyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Team6OrderSyncWorker> _logger;
        private readonly TimeSpan _interval;
        private readonly Team6IntegrationSettings _settings;

        public Team6OrderSyncWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<Team6OrderSyncWorker> logger,
            IOptions<Team6IntegrationSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _settings = settings.Value;
            _interval = TimeSpan.FromSeconds(Math.Max(5, _settings.PollingIntervalSeconds));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("Team6 sync worker is disabled.");
                return;
            }

            _logger.LogInformation("Team6 sync worker started. Polling every {Seconds} seconds.", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<ITeam6OrderSyncService>();

                    var syncedCount = await syncService.SyncActiveOrdersAsync(stoppingToken);

                    if (syncedCount > 0)
                    {
                        _logger.LogInformation("Team6 sync completed. Imported {Count} new orders.", syncedCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while syncing Team6 active orders.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
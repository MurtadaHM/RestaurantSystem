using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;

namespace RestaurantSystem.Infrastructure.BackgroundServices
{
    public class ReservationStatusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationStatusWorker> _logger;

        public ReservationStatusWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ReservationStatusWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Started: يراقب الآن الحجوزات وحالات التوصيل الخارجية...");

            // مهم جدًا على Render: نعطي الداتابيس وقت حتى تصير جاهزة
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdatedTableStatuses(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "ReservationStatusWorker: database may not be ready yet or a transient error occurred. Will retry in the next cycle.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckAndUpdatedTableStatuses(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();

            var todayReservations = await reservationRepository.GetTodayReservationsAsync();

            // خليه نفس منطقك الحالي هنا
            foreach (var reservation in todayReservations)
            {
                // TODO: احتفظ بمنطقك السابق كما هو
                // مثال:
                // var table = await tableRepository.GetByIdAsync(reservation.TableId);
                // if (table == null) continue;
                // ...
            }
        }
    }
}
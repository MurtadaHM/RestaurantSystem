using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.Contracts.Signals;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.BackgroundServices
{
    public class ReservationStatusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationStatusWorker> _logger;

        public ReservationStatusWorker(IServiceScopeFactory scopeFactory, ILogger<ReservationStatusWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Started: يراقب الآن الحجوزات وحالات التوصيل الخارجية...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdatedTableStatuses();
                    await UpdateExternalDeliveryStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "حدث خطأ أثناء عمل الـ Worker");
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // زدنا الوقت قليلاً لتوفير استهلاك الـ API
            }
        }

        private async Task UpdateExternalDeliveryStatuses()
        {
            using var scope = _scopeFactory.CreateScope();
            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var sendyService = scope.ServiceProvider.GetRequiredService<ISendyIntegrationService>();
            var notification = scope.ServiceProvider.GetRequiredService<IOrderNotificationService>();

            var activeDeliveries = await orderRepo.GetAllAsync();
            var filteredOrders = activeDeliveries.Where(o => o.IsSyncedToExternalProvider
                                                        && o.Status != OrderStatus.Completed
                                                        && o.Status != OrderStatus.Cancelled);

            foreach (var order in filteredOrders)
            {
                if (order.ExternalOrderId.HasValue)
                {
                    // ✅ 1. تصحيح اسم الميثود إلى GetDeliveryStatusAsync (حسب ما عرفناه في ISendyIntegrationService)
                    // ✅ 2. استقبال الـ Tuple وتحديد الحالة الداخلية (InternalStatus)
                    var deliveryInfo = await sendyService.GetDeliveryStatusAsync(order.ExternalOrderId.Value);
                    var latestStatus = deliveryInfo.InternalStatus;

                    if (latestStatus != order.ExternalDeliveryStatus)
                    {
                        _logger.LogInformation("🚚 تحديث حالة توصيل الطلب {Num}: {Status}", order.OrderNumber, latestStatus);

                        order.ExternalDeliveryStatus = latestStatus;
                        order.LastExternalSyncDate = DateTime.UtcNow;

                        if (latestStatus == DeliveryPartnerStatus.Delivered)
                        {
                            order.Status = OrderStatus.Completed;
                            order.CompletedAt = DateTime.UtcNow;
                        }

                        await orderRepo.UpdateAsync(order);

                        // ✅ 3. تصحيح التنبيه: أضفنا order.OrderNumber كباراميتر ثالث كما يطلب الـ Interface المحدث
                        await notification.NotifyOrderStatusChangedAsync(order.Id, order.OrderNumber, order.Status.ToString());
                    }
                }
            }
        }

        private async Task CheckAndUpdatedTableStatuses()
        {
            using var scope = _scopeFactory.CreateScope();
            var tableRepo = scope.ServiceProvider.GetRequiredService<ITableRepository>();
            var resRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

            var now = DateTime.Now;
            var alertWindow = now.AddMinutes(60);

            var todayReservations = await resRepo.GetTodayReservationsAsync();

            foreach (var res in todayReservations)
            {
                bool isUpcoming = res.ReservationDate <= alertWindow && res.ReservationDate >= now.AddMinutes(-15);

                if (isUpcoming && res.Status == ReservationStatus.Confirmed)
                {
                    var table = await tableRepo.GetByIdAsync(res.TableId);
                    if (table != null && table.Status == TableStatus.Available)
                    {
                        _logger.LogInformation("🟡 حجز قريب: الطاولة {Num} -> Reserved", table.TableNumber);
                        await tableRepo.UpdateStatusAsync(table.Id, TableStatus.Reserved);
                    }
                }
                else if (res.ReservationDate < now.AddMinutes(-30) && res.Status == ReservationStatus.Confirmed)
                {
                    var table = await tableRepo.GetByIdAsync(res.TableId);
                    if (table != null && table.Status == TableStatus.Reserved)
                    {
                        _logger.LogInformation("🟢 تحرير الطاولة {Num} (No-Show)", table.TableNumber);
                        await tableRepo.UpdateStatusAsync(table.Id, TableStatus.Available);
                    }
                }
            }
        }
    }
}
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Configurations;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class Team6OrderSyncService : ITeam6OrderSyncService
    {
        private readonly ITeam6IntegrationService _team6IntegrationService;
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly Team6IntegrationSettings _settings;

        public Team6OrderSyncService(
            ITeam6IntegrationService team6IntegrationService,
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IOptions<Team6IntegrationSettings> settings)
        {
            _team6IntegrationService = team6IntegrationService;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _settings = settings.Value;
        }

        public async Task<int> SyncActiveOrdersAsync(CancellationToken cancellationToken = default)
        {
            var result = await _team6IntegrationService.GetActiveOrdersAsync(cancellationToken);

            if (result == null || !result.Success || result.Data == null || result.Data.Count == 0)
                return 0;

            if (_settings.FallbackUserId == Guid.Empty)
                throw new InvalidOperationException("Team6Integration:FallbackUserId is missing or invalid.");

            var createdCount = 0;

            foreach (var partnerOrder in result.Data.Where(x => x.IsActive))
            {
                var existing = await _orderRepository.GetByPartnerOrderIdAsync(partnerOrder.OrderId, "Team6");
                if (existing != null)
                    continue;

                var allOrders = await _orderRepository.GetAllAsync();
                int nextOrderNumber = (allOrders.Any() ? allOrders.Max(o => o.OrderNumber) : 0) + 1;

                var localTable = await _tableRepository.GetByIdAsync(partnerOrder.TableId);

                var localOrder = new Order
                {
                    OrderNumber = nextOrderNumber,
                    UserId = _settings.FallbackUserId,
                    TableId = localTable?.Id,
                    OrderType = OrderType.DineIn,
                    Status = OrderStatus.Pending,
                    TotalAmount = partnerOrder.TotalPrice,
                    SpecialNotes = $"Imported from Team6 - Restaurant: {partnerOrder.RestaurantName} - Team6TableNumber: {partnerOrder.TableNumber}",
                    CreatedAt = partnerOrder.CreatedAt,
                    UpdatedAt = partnerOrder.UpdatedAt,
                    PartnerOrderId = partnerOrder.OrderId,
                    PartnerRestaurantId = partnerOrder.RestaurantId,
                    PartnerUserId = partnerOrder.UserId,
                    PartnerSource = "Team6",
                    LastPartnerSyncDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                await _orderRepository.AddAsync(localOrder);
                createdCount++;
            }

            return createdCount;
        }
    }
}
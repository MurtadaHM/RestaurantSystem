using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Integrations.Team6;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class Team6TrackingService : ITeam6TrackingService
    {
        private readonly IOrderRepository _orderRepository;

        public Team6TrackingService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Team6OrderTrackingResponseDto> GetOrderTrackingAsync(string partnerOrderId)
        {
            var order = await _orderRepository.GetByPartnerOrderIdAsync(partnerOrderId, "Team6");

            if (order == null)
                throw new Exception("الطلب غير موجود");

            var mappedStatus = MapToTeam6Status(order);

            return new Team6OrderTrackingResponseDto
            {
                OrderId = order.PartnerOrderId ?? string.Empty,
                RestaurantId = order.PartnerRestaurantId ?? string.Empty,
                TableId = order.TableId,
                TableNumber = order.Table?.TableNumber ?? string.Empty,
                UserId = order.PartnerUserId,
                Status = mappedStatus,
                CreatedAtUtc = order.CreatedAt,
                UpdatedAtUtc = order.UpdatedAt ?? order.CreatedAt,
                IsActive = mappedStatus != "SessionClosed"
            };
        }

        private static string MapToTeam6Status(Order order)
        {
            // إذا الويتر حرر الطاولة / صارت Available
            if (order.Table != null && order.Table.Status == TableStatus.Available)
                return "SessionClosed";

            return order.Status switch
            {
                OrderStatus.Pending => "Received",
                OrderStatus.Confirmed => "Received",
                OrderStatus.Preparing => "Cooking",
                OrderStatus.Ready => "ReadyToServe",
                OrderStatus.Completed => "Served",
                _ => "Received"
            };
        }
    }
}
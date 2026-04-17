using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Integrations.Team6;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/team6/orders")]
    [Produces("application/json")]
    [Tags("Public Team6 Orders")]
    public class PublicTeam6OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public PublicTeam6OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Endpoint بسيط للحالة فقط (للتوافق مع النسخة القديمة)
        /// </summary>
        [HttpGet("{partnerOrderId}/status")]
        public async Task<ActionResult<ApiResponse<Team6OrderStatusResponseDto>>> GetOrderStatus(string partnerOrderId)
        {
            var order = await _orderRepository.GetByPartnerOrderIdAsync(partnerOrderId, "Team6");

            if (order == null)
            {
                return NotFound(ApiResponse<Team6OrderStatusResponseDto>.Fail("الطلب غير موجود"));
            }

            var response = new Team6OrderStatusResponseDto
            {
                PartnerOrderId = order.PartnerOrderId ?? string.Empty,
                InternalOrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = MapToTeam6Status(order),
                ExternalDeliveryStatus = order.ExternalDeliveryStatus.ToString(),
                UpdatedAtUtc = order.UpdatedAt ?? order.CreatedAt,
                Notes = order.SpecialNotes
            };

            return Ok(ApiResponse<Team6OrderStatusResponseDto>.Ok(response));
        }

        /// <summary>
        /// Endpoint تتبع كامل للطلب حسب partnerOrderId
        /// هذا هو الأنسب لربط Team 6 polling كل 5 ثواني
        /// </summary>
        [HttpGet("{partnerOrderId}/tracking")]
        public async Task<ActionResult<ApiResponse<Team6OrderTrackingResponseDto>>> GetOrderTracking(string partnerOrderId)
        {
            var order = await _orderRepository.GetByPartnerOrderIdAsync(partnerOrderId, "Team6");

            if (order == null)
            {
                return NotFound(ApiResponse<Team6OrderTrackingResponseDto>.Fail("الطلب غير موجود"));
            }

            var mappedStatus = MapToTeam6Status(order);

            var response = new Team6OrderTrackingResponseDto
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

            return Ok(ApiResponse<Team6OrderTrackingResponseDto>.Ok(response));
        }

        /// <summary>
        /// جلب تاريخ طلبات مستخدم معين داخل مطعم معين
        /// </summary>
        [HttpGet("/api/public/team6/restaurants/{partnerRestaurantId}/users/{partnerUserId}/orders")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Team6UserOrderHistoryItemDto>>>> GetUserOrders(
            string partnerRestaurantId,
            string partnerUserId)
        {
            var orders = await _orderRepository.GetByPartnerUserIdAsync(
                partnerUserId,
                partnerRestaurantId,
                "Team6");

            var response = orders.Select(order => new Team6UserOrderHistoryItemDto
            {
                PartnerOrderId = order.PartnerOrderId ?? string.Empty,
                PartnerUserId = order.PartnerUserId,
                PartnerRestaurantId = order.PartnerRestaurantId,
                InternalOrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = MapToTeam6Status(order),
                ExternalDeliveryStatus = order.ExternalDeliveryStatus.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAtUtc = order.CreatedAt,
                UpdatedAtUtc = order.UpdatedAt ?? order.CreatedAt,
                Notes = order.SpecialNotes
            });

            return Ok(ApiResponse<IEnumerable<Team6UserOrderHistoryItemDto>>.Ok(response));
        }

        private static string MapToTeam6Status(Order order)
        {
            // إذا الويتر حرر الطاولة وصارت Available نعتبر الجلسة منتهية
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
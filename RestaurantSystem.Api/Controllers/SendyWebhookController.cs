using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.DTOs.External;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy;

namespace RestaurantSystem.Api.Controllers.Webhooks
{
    [ApiController]
    [Route("api/webhooks/sendy")]
    [Produces("application/json")]
    public class SendyWebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<SendyWebhookController> _logger;
        private readonly SendyClientSettings _settings;

        public SendyWebhookController(
            IOrderService orderService,
            ILogger<SendyWebhookController> logger,
            IOptions<SendyClientSettings> settings)
        {
            _orderService = orderService;
            _logger = logger;
            _settings = settings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveUpdate([FromBody] SendyWebhookPayload? payload)
        {
            if (payload is null)
            {
                _logger.LogWarning("⚠️ Empty Sendy webhook payload received.");
                return BadRequest(new
                {
                    success = false,
                    message = "Webhook payload is required."
                });
            }

            if (!Request.Headers.TryGetValue("X-Webhook-Secret", out var receivedSecret) ||
                string.IsNullOrWhiteSpace(_settings.WebhookSecret) ||
                !string.Equals(receivedSecret.ToString(), _settings.WebhookSecret, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "🚨 Unauthorized webhook access attempt from IP: {Ip}",
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid webhook secret."
                });
            }

            try
            {
                _logger.LogInformation(
                    "🔔 Webhook received for external order {OrderId}. EventType: {EventType}, NewStatus: {Status}",
                    payload.OrderId,
                    payload.EventType,
                    payload.NewStatus);

                var updatedOrder = await _orderService.UpdateExternalStatusFromWebhookAsync(
                    payload.OrderId,
                    payload.NewStatus,
                    payload.CourierName,
                    payload.CourierPhone,
                    payload.TrackingUrl);

                _logger.LogInformation(
                    "✅ Webhook processed successfully for local order #{OrderNumber}. External status: {ExternalStatus}",
                    updatedOrder.OrderNumber,
                    updatedOrder.ExternalDeliveryStatus);

                return Ok(new
                {
                    success = true,
                    message = "Webhook processed successfully.",
                    orderId = updatedOrder.Id,
                    orderNumber = updatedOrder.OrderNumber,
                    externalOrderId = updatedOrder.ExternalOrderId,
                    externalStatus = updatedOrder.ExternalDeliveryStatus,
                    trackingUrl = updatedOrder.TrackingUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "🚨 Webhook processing failed for external order {OrderId}",
                    payload.OrderId);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Internal server error while processing webhook."
                });
            }
        }
    }
}
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Contracts.ExternalServices;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Integrations;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.ExternalServices.Delivery.Sendy
{
    public class SendyIntegrationService : ISendyIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly SendyClientSettings _settings;
        private readonly ILogger<SendyIntegrationService> _logger;

        public SendyIntegrationService(
            HttpClient httpClient,
            IOptions<SendyClientSettings> settings,
            ILogger<SendyIntegrationService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            if (_httpClient.BaseAddress == null &&
                Uri.TryCreate(_settings.BaseUrl, UriKind.Absolute, out var baseUri))
            {
                _httpClient.BaseAddress = baseUri;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey.Trim());
            }
            else
            {
                _logger.LogWarning("Sendy API key is missing.");
            }
        }

        public async Task<(bool Success, Guid? ExternalId, string? ExternalPublicId, string? TrackingUrl, string Message)>
            PushOrderToSendyAsync(IntegrationPushOrderRequest request)
        {
            try
            {
                var endpoint = $"{_settings.AdminRoute}/orders";

                // Use provided values or fall back to configured defaults
                var addressProvinceCode = request.AddressProvinceCode ?? _settings.DefaultAddressProvinceCode;
                var addressAreaId = request.AddressAreaId ?? _settings.DefaultAddressAreaId;

                // Validate required address reference fields before calling Sendy
                if (string.IsNullOrWhiteSpace(addressProvinceCode))
                {
                    var msg = "AddressProvinceCode is required (request or Sendy default).";
                    _logger.LogWarning("Sendy push validation failed: {Msg} OrderNumber: {OrderNo}", msg, request.OrderNumber);
                    return (false, null, null, null, msg);
                }

                if (!addressAreaId.HasValue || addressAreaId == Guid.Empty)
                {
                    var msg = "AddressAreaId must not be null or empty GUID (request or Sendy default).";
                    _logger.LogWarning("Sendy push validation failed: {Msg} OrderNumber: {OrderNo}", msg, request.OrderNumber);
                    return (false, null, null, null, msg);
                }

                var payload = new
                {
                    customerName = request.CustomerName,
                    customerPhone = request.CustomerPhone,
                    customerAddress = request.CustomerAddress,
                    deliveryLat = request.DeliveryLat,
                    deliveryLng = request.DeliveryLng,
                    orderValue = request.OrderValue,
                    deliveryFee = request.DeliveryFee,
                    externalRef = request.ExternalRef,
                    fulfillmentType = request.FulfillmentType,
                    deliveryMode = request.DeliveryMode,
                    paymentMethod = request.PaymentMethod,
                    // new address reference fields required by Sendy
                    addressProvinceCode = addressProvinceCode,
                    addressAreaId = addressAreaId.Value
                };

                _logger.LogInformation("Sending order {OrderNo} to Sendy using X-API-Key", request.OrderNumber);

                using var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Sendy response status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Sendy response body: {Body}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, null, null, $"Sendy Error {response.StatusCode}: {responseBody}");
                }

                var result = JsonSerializer.Deserialize<SendyResponseEnvelope<IntegrationOrderResponse>>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return (
                    true,
                    result?.Data?.Id,
                    result?.Data?.PublicId,
                    null,
                    "Success"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error while pushing order to Sendy");
                return (false, null, null, null, ex.Message);
            }
        }

        public async Task<(DeliveryPartnerStatus InternalStatus, string ExternalStatus, string DriverName, string DriverPhone, string? TrackingUrl)>
            GetDeliveryStatusAsync(Guid externalOrderId)
        {
            try
            {
                var endpoint = $"{_settings.AdminRoute}/orders/{externalOrderId}";
                using var response = await _httpClient.GetAsync(endpoint);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get Sendy delivery status. Status: {Status}", response.StatusCode);
                    return (DeliveryPartnerStatus.Idle, "unknown", string.Empty, string.Empty, null);
                }

                var result = JsonSerializer.Deserialize<SendyResponseEnvelope<IntegrationOrderResponse>>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var data = result?.Data;

                // Use centralized mapping
                var mapped = SendyStatusMapper.MapToDeliveryPartnerStatus(data?.Status);

                return (
                    mapped,
                    data?.Status ?? "unknown",
                    data?.CourierName ?? string.Empty,
                    data?.CourierPhone ?? string.Empty,
                    null
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Sendy delivery status for {ExternalOrderId}", externalOrderId);
                return (DeliveryPartnerStatus.Idle, "error", string.Empty, string.Empty, null);
            }
        }

        public async Task<bool> CancelOrderAsync(Guid externalOrderId, string reason)
        {
            try
            {
                var endpoint = $"{_settings.AdminRoute}/orders/{externalOrderId}/cancel";
                using var response = await _httpClient.PostAsJsonAsync(endpoint, new { reason });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling Sendy order {ExternalOrderId}", externalOrderId);
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(Guid externalOrderId, string status)
        {
            try
            {
                var endpoint = $"{_settings.AdminRoute}/orders/{externalOrderId}";
                using var response = await _httpClient.PutAsJsonAsync(endpoint, new { status });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Sendy order status {ExternalOrderId}", externalOrderId);
                return false;
            }
        }

        // remove/reuse previous MapToInternalStatus - replace its body with call to mapper
        private static DeliveryPartnerStatus MapToInternalStatus(string? externalStatus)
        {
            return SendyStatusMapper.MapToDeliveryPartnerStatus(externalStatus);
        }
    }

    public class SendyResponseEnvelope<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    public class IntegrationOrderResponse
    {
        public Guid Id { get; set; }
        public string? PublicId { get; set; }
        public string? ExternalRef { get; set; }
        public string? Status { get; set; }
        public string? FulfillmentType { get; set; }
        public string? CourierName { get; set; }
        public string? CourierPhone { get; set; }
    }
}
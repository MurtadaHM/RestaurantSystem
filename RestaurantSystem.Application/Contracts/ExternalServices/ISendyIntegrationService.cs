using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.ExternalServices
{
    public interface ISendyIntegrationService
    {
        Task<(bool Success, Guid? ExternalId, string? ExternalPublicId, string? TrackingUrl, string Message)>
            PushOrderToSendyAsync(IntegrationPushOrderRequest request);

        Task<(DeliveryPartnerStatus InternalStatus, string ExternalStatus, string DriverName, string DriverPhone, string? TrackingUrl)>
            GetDeliveryStatusAsync(Guid externalOrderId);

        Task<bool> CancelOrderAsync(Guid externalOrderId, string reason);

        Task<bool> UpdateStatusAsync(Guid externalOrderId, string status);
    }
}
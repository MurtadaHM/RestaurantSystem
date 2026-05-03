namespace RestaurantSystem.Application.Integrations
{
    using System;
    using RestaurantSystem.Domain.Enums;

    /// <summary>
    /// Centralized mapping from Sendy/provider status strings to DeliveryPartnerStatus.
    /// Used by both SendyIntegrationService and OrderService.
    /// </summary>
    public static class SendyStatusMapper
    {
        public static DeliveryPartnerStatus MapToDeliveryPartnerStatus(string? externalStatus)
        {
            if (string.IsNullOrWhiteSpace(externalStatus))
                return DeliveryPartnerStatus.Idle;

            return externalStatus.Trim().ToLowerInvariant() switch
            {
                "pending" => DeliveryPartnerStatus.SearchingForDriver,
                "searching" => DeliveryPartnerStatus.SearchingForDriver,
                "confirmed" or "accepted" or "assigned" => DeliveryPartnerStatus.DriverAssigned,
                "at_store" or "atstore" or "at_pickup" or "atpickup" or "arrived_at_store" => DeliveryPartnerStatus.AtStore,
                "pickedup" or "picked_up" => DeliveryPartnerStatus.PickedUp,
                "intransit" or "in_transit" => DeliveryPartnerStatus.InTransit,
                "at_destination" or "atdestination" or "arrived" or "arrived_at_customer" or "arrivedatcustomer" => DeliveryPartnerStatus.ArrivedAtCustomer,
                "delivered" or "completed" => DeliveryPartnerStatus.Delivered,
                "failed" or "delivery_exception" or "deliveryexception" => DeliveryPartnerStatus.Failed,
                "cancelled" or "canceled" => DeliveryPartnerStatus.Cancelled,
                "returned" => DeliveryPartnerStatus.Returned,
                _ => DeliveryPartnerStatus.Idle
            };
        }
    }
}
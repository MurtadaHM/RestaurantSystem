using System;
using System.Collections.Generic;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? TableId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int OrderNumber { get; set; }
        public OrderType OrderType { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal? DeliveryFee { get; set; }

        public string? SpecialNotes { get; set; }
        public DateTime? ExpectedReadyTime { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool IsStockDeducted { get; set; } = false;

        // Sendy Integration
        public Guid? ExternalOrderId { get; set; }
        public string? ExternalPublicId { get; set; }
        public DeliveryPartnerStatus ExternalDeliveryStatus { get; set; } = DeliveryPartnerStatus.Idle;
        public bool IsSyncedToExternalProvider { get; set; } = false;
        public DateTime? LastExternalSyncDate { get; set; }
        public string? TrackingUrl { get; set; }
        public string? DeliveryAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CourierName { get; set; }
        public string? CourierPhoneNumber { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Table? Table { get; set; }

      

        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }

        // NEW: department-level progress
        public ICollection<OrderDepartmentProgress> OrderDepartmentProgresses { get; set; } = new List<OrderDepartmentProgress>();

        // Team 6
        public string? PartnerUserId { get; set; }
        public string? PartnerOrderId { get; set; }
        public string? PartnerSource { get; set; }
        public string? PartnerRestaurantId { get; set; }
        public DateTime? LastPartnerSyncDate { get; set; }
    }
}
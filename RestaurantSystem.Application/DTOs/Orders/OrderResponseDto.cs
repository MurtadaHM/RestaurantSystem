using System;
using System.Collections.Generic;

namespace RestaurantSystem.Application.DTOs.Orders
{
    /// <summary>
    /// DTO لاستجابة الطلب - هو الكائن الذي يراه الزبون أو موظف الكاشير
    /// </summary>
    public class OrderResponseDto
    {
        public Guid Id { get; set; }

        /// <summary>رقم الطلب البسيط (مثل 101, 102)</summary>
        public int OrderNumber { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public Guid? TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;

        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }

        public string SpecialNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // ──────────────────────────────────────────────────────────
        // Sendy Integration
        // ──────────────────────────────────────────────────────────

        /// <summary>المعرف الخارجي الداخلي للطلب في Sendy</summary>
        public Guid? ExternalOrderId { get; set; }

        /// <summary>المعرف العام الخارجي القادم من Sendy مثل ORD-20260410-XXXXX</summary>
        public string? ExternalPublicId { get; set; }

        public string ExternalDeliveryStatus { get; set; } = string.Empty;
        public bool IsSyncedToExternalProvider { get; set; }

        public string? DeliveryAddress { get; set; }
        public string? CustomerPhoneNumber { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? CourierName { get; set; }
        public string? CourierPhoneNumber { get; set; }

        /// <summary>رابط التتبع الخارجي إن توفر من Sendy</summary>
        public string? TrackingUrl { get; set; }

        /// <summary>آخر وقت مزامنة مع Sendy</summary>
        public DateTime? LastExternalSyncDate { get; set; }

        // ──────────────────────────────────────────────────────────
        // Team 6 Integration
        // ──────────────────────────────────────────────────────────
        public string? PartnerUserId { get; set; }
        public string? PartnerOrderId { get; set; }
        public string? PartnerSource { get; set; }
        public string? PartnerRestaurantId { get; set; }
        public DateTime? LastPartnerSyncDate { get; set; }

        // ──────────────────────────────────────────────────────────

        public List<OrderItemResponseDto> Items { get; set; } = new();

        /// <summary>
        /// معلومات الدفع إن كانت موجودة ومحملة مع الطلب
        /// </summary>
        public PaymentResponseDto? Payment { get; set; }
    }

    public class OrderItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string SpecialInstructions { get; set; } = string.Empty;
    }

    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}
namespace RestaurantSystem.Application.DTOs.Orders
{
    /// <summary>
    /// DTO لاستجابة الطلب
    /// </summary>
    public class OrderResponseDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = default!;

        public Guid? TableId { get; set; }

        public string TableNumber { get; set; } = default!;

        public string OrderType { get; set; } = default!;

        public string Status { get; set; } = default!;

        public decimal TotalAmount { get; set; }

        public decimal DeliveryFee { get; set; }

        public string SpecialNotes { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();

        public PaymentResponseDto Payment { get; set; } = default!;
    }

    /// <summary>
    /// DTO لعنصر الطلب في الاستجابة
    /// </summary>
    public class OrderItemResponseDto
    {
        public Guid Id { get; set; }

        public Guid MenuItemId { get; set; }

        public string MenuItemName { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice => Price * Quantity;

        public string SpecialInstructions { get; set; } = default!;
    }

    /// <summary>
    /// DTO للدفع في الاستجابة
    /// </summary>
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = default!;

        public string PaymentMethod { get; set; } = default!;

        public string TransactionReference { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}

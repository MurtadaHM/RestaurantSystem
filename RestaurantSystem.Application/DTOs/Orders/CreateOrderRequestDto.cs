// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// DTOs/Orders/CreateOrderRequestDto.cs
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    public class CreateOrderRequestDto
    {
        public Guid UserId { get; set; }
        public Guid? TableId { get; set; }

        // ✅ تم التحديث من Type إلى OrderType لتطابق الـ Service
        public OrderType OrderType { get; set; }

        public string? SpecialNotes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
        public decimal DeliveryFee { get; set; } = 0;
    }

    public class CreateOrderItemDto
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}
namespace RestaurantSystem.Application.DTOs.Inventory
{
    public class StockMovementResponseDto
    {
        public Guid Id { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty; // Purchase, Sale...
        public string? Reason { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
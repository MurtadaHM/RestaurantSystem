namespace RestaurantSystem.Application.DTOs.Inventory
{
    public class AddStockRequestDto
    {
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; } // مثل "شراء من مجهز كربلاء"
    }
}
namespace RestaurantSystem.Application.DTOs.Inventory
{
    public class MenuItemIngredientDto
    {
        public Guid IngredientId { get; set; }
        public string? IngredientName { get; set; } // للعرض فقط
        public decimal Quantity { get; set; } // الكمية المطلوبة لهذا الصنف
    }
}
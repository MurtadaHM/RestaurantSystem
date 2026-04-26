namespace RestaurantSystem.Domain.Entities
{
    public class MenuItemIngredient : BaseEntity
    {
        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        public decimal Quantity { get; set; }

        public string? Notes { get; set; }
        public bool IsOptional { get; set; } = false;
        public decimal WastePercentage { get; set; } = 0;
    }
}
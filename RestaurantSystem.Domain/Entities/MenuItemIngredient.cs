namespace RestaurantSystem.Domain.Entities
{
    public class MenuItemIngredient
    {
        // الربط مع صنف الطعام (مثلاً: كباب عراقي)
        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        // الربط مع المادة الأولية (مثلاً: لحم غنم)
        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        // الكمية المطلوبة لهذا الصنف (مثلاً: 0.200 إذا كانت الوحدة كغم)
        public decimal Quantity { get; set; }
        public Guid Id { get; set; } // أضف هذا السطر
        public DateTime CreatedAt { get; set; } // وأضف هذا السطر
    }
}
namespace RestaurantSystem.Application.DTOs.Inventory
{
    public class IngredientResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal MinThreshold { get; set; }
        public string Unit { get; set; } = string.Empty; // سنحول الـ Enum لنص هنا
        public decimal UnitPrice { get; set; }
        public bool IsLowStock => CurrentStock <= MinThreshold; // تنبيه ذكي بالواجهة
    }
}
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Inventory
{
    public class CreateIngredientRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal InitialStock { get; set; }
        public decimal MinThreshold { get; set; }
        public UnitType Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
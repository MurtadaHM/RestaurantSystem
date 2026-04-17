using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Ingredient : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        // الكمية المتوفرة حالياً في المخزن
        public decimal CurrentStock { get; set; }

        // الحد الأدنى الذي عنده يبدأ النظام بإرسال تنبيهات (Alerts)
        public decimal MinThreshold { get; set; }

        public UnitType Unit { get; set; }

        // سعر التكلفة للوحدة الواحدة (لحساب تكلفة الأطباق لاحقاً)
        public decimal UnitPrice { get; set; }

        // الربط مع الوصفات (كم مادة تدخل في كم صنف)
        public ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();

        // سجل حركات هذه المادة
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}
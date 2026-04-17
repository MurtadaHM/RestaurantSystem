using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class StockMovement : BaseEntity
    {
        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        // الكمية التي تحركت (موجبة في الشراء، سالبة في البيع)
        public decimal Quantity { get; set; }

        public MovementType Type { get; set; }

        // سبب الحركة (رقم الأوردر، رقم فاتورة الشراء، أو ملاحظة يدوية)
        public string? Reason { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.UtcNow;

        // من هو المستخدم الذي قام بالحركة (المدير أو النادل)
        public Guid? UserId { get; set; }
    }
}
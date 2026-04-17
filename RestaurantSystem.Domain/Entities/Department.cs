using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // مطبخ، باربستا، إلخ
        public string? Description { get; set; }        // وصف بسيط للقسم

        // أيقونة للواجهة (مثلاً: "fa-coffee" أو "kitchen-icon")
        public string? Icon { get; set; }

        public DepartmentStatus Status { get; set; } = DepartmentStatus.Active;

        // الربط مع المنيو (العلاقة: القسم الواحد يحتوي على عدة أصناف)
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        // الربط مع تذاكر الطلبات (لأجل شاشة الشيف)
        // public virtual ICollection<OrderTicket> OrderTickets { get; set; }
    }
}
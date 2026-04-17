using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class Table : BaseEntity
    {
        public string TableNumber { get; set; } = string.Empty;

        // كود عام يستخدم داخل QR
        public string Code { get; set; } = string.Empty;

        public int Capacity { get; set; }

        // لازم يكون إجباري
        public string Location { get; set; } = string.Empty;

        // مثال: Main Hall / VIP / Garden
        public string? Zone { get; set; }

        public int? FloorNumber { get; set; }

        public bool IsActive { get; set; } = true;

        // للتحكم بإمكانية الطلب من QR
        public bool IsOrderingEnabled { get; set; } = true;

        public TableStatus Status { get; set; } = TableStatus.Available;

        public string? Notes { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class OrderDepartmentProgress : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }

        public OrderDepartmentStatus Status { get; set; } = OrderDepartmentStatus.Pending;

        public DateTime? StartedAt { get; set; }
        public DateTime? ReadyAt { get; set; }

        public string? Notes { get; set; }
    }
}
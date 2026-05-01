using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Orders
{
    public class OrderDepartmentProgressDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public OrderDepartmentStatus Status { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? ReadyAt { get; set; }

        public string? Notes { get; set; }
    }
}
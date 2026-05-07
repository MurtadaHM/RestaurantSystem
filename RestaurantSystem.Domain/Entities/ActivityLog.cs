using System;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class ActivityLog : BaseEntity
    {
        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserRole { get; set; }

        public ActivityActionType ActionType { get; set; }

        public string Module { get; set; } = string.Empty;

        public string? EntityName { get; set; }

        public Guid? EntityId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
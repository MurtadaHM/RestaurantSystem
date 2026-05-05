using System.Collections.Generic;

namespace RestaurantSystem.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string? FullName { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}  
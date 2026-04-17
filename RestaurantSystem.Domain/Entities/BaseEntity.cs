namespace RestaurantSystem.Domain.Entities
{
    public abstract class BaseEntity
    {
        // ✅ الحل: احذف الـ = Guid.NewGuid() 
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
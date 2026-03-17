using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("Orders");

            // ──────────────────────────────────────────
            // Primary Key
            // ──────────────────────────────────────────
            builder.HasKey(o => o.Id);

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>()              // ✅ Enum → string
                .HasColumnType("text")
                .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.OrderType)
                .IsRequired()
                .HasConversion<string>()              // ✅ Enum → string
                .HasColumnType("text");

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DeliveryFee)
                .IsRequired(false)                    // ✅ decimal? nullable
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.SpecialNotes)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(o => o.ExpectedReadyTime)
                .IsRequired(false)                    // ✅ DateTime? nullable
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.CompletedAt)
                .IsRequired(false)                    // ✅ DateTime? nullable
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.UpdatedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            builder.Property(o => o.DeletedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────
            builder.HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_UserId");

            builder.HasIndex(o => o.TableId)
                .HasDatabaseName("IX_Orders_TableId");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            // ──────────────────────────────────────────
            // Relationships
            // ──────────────────────────────────────────

            // ✅ Order → User (Many-to-One)
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);   // ✅ نمنع حذف User لو عنده Orders

            // ✅ Order → Table (Many-to-One) Optional
            builder.HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.SetNull)     // ✅ لو حذفنا Table نجعل TableId = null
                .IsRequired(false);

            // ✅ Order → Payment (One-to-One)  — configured from Payment side
            // ✅ Order → OrderItems (One-to-Many) — configured from OrderItem side

            // ──────────────────────────────────────────
            // Soft Delete Filter
            // ──────────────────────────────────────────
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}

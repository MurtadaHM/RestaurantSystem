using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class OrderDepartmentProgressConfiguration : IEntityTypeConfiguration<OrderDepartmentProgress>
    {
        public void Configure(EntityTypeBuilder<OrderDepartmentProgress> builder)
        {
            builder.ToTable("OrderDepartmentProgresses");

            // Primary Key
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            // Foreign Keys
            builder.Property(x => x.OrderId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.DepartmentId)
                .IsRequired()
                .HasColumnType("uuid");

            // Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            // Optional fields
            builder.Property(x => x.StartedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.ReadyAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            // BaseEntity fields
            builder.Property(x => x.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // Prevent duplicate progress row for same order + department
            builder.HasIndex(x => new { x.OrderId, x.DepartmentId })
                .IsUnique()
                .HasDatabaseName("IX_OrderDepartmentProgress_OrderId_DepartmentId");

            // Relationships
            builder.HasOne(x => x.Order)
                .WithMany(o => o.OrderDepartmentProgresses)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Department)
                .WithMany(d => d.OrderDepartmentProgresses)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
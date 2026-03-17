using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("Payments");

            // ──────────────────────────────────────────
            // Primary Key — PostgreSQL uuid
            // ──────────────────────────────────────────
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()         // ✅ PostgreSQL uuid auto-generate
                .HasColumnType("uuid");

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(p => p.OrderId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()       // ✅ Enum → string (text)
                .HasColumnType("text");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()       // ✅ Enum → string (text)
                .HasColumnType("text")
                .ValueGeneratedOnAdd()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(p => p.TransactionReference)
                .HasMaxLength(100)             // ✅ 100 وليس 200
                .HasColumnType("character varying(100)");

            builder.Property(p => p.Notes)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(p => p.PaymentDate)
                .HasColumnType("timestamp with time zone"); // ✅ nullable

            builder.Property(p => p.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("timestamp with time zone"); // ✅ nullable DateTime?

            builder.Property(p => p.DeletedAt)
                .HasColumnType("timestamp with time zone"); // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────

            // ✅ كل طلب له دفع واحد فقط — One-to-One
            builder.HasIndex(p => p.OrderId)
                .IsUnique()
                .HasDatabaseName("IX_Payments_OrderId");

            // ──────────────────────────────────────────
            // Relationships
            // ──────────────────────────────────────────

            // ✅ Payment → Order (One-to-One)
            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("MenuItems");

            // ──────────────────────────────────────────
            // Primary Key
            // ──────────────────────────────────────────
            builder.HasKey(m => m.Id);

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)");

            builder.Property(m => m.Description)
                .IsRequired(false)
                .HasMaxLength(1000)
                .HasColumnType("character varying(1000)");

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.ImageUrl)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(m => m.Ingredients)
                .IsRequired(false)
                .HasMaxLength(1000)
                .HasColumnType("character varying(1000)");

            builder.Property(m => m.Calories)
                .IsRequired(false)
                .HasColumnType("integer");

            builder.Property(m => m.IsAvailable)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            builder.Property(m => m.PreparationTimeMinutes)
                .HasColumnType("integer")
                .HasDefaultValue(15);

            builder.Property(m => m.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(m => m.UpdatedAt)
                .HasColumnType("timestamp with time zone");   // ✅ nullable

            builder.Property(m => m.DeletedAt)
                .HasColumnType("timestamp with time zone");   // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────
            builder.HasIndex(m => m.CategoryId)
                .HasDatabaseName("IX_MenuItems_CategoryId");

            // ──────────────────────────────────────────
            // Relationships
            // ──────────────────────────────────────────

            // ✅ MenuItem → Category (Many-to-One)
            builder.HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);   // ✅ نمنع حذف Category لو فيها Items

            // ──────────────────────────────────────────
            // Computed Properties — Ignored
            // ──────────────────────────────────────────
            // لا يوجد Computed Properties في MenuItem

            // ──────────────────────────────────────────
            // Soft Delete Filter
            // ──────────────────────────────────────────
            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}

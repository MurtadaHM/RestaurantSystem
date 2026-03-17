using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("Categories");

            // ──────────────────────────────────────────
            // Primary Key — PostgreSQL يولّد الـ uuid تلقائياً
            // ──────────────────────────────────────────
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()         // ✅ PostgreSQL uuid auto-generate
                .HasColumnType("uuid");

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500)             // ✅ 500 وليس 1000
                .HasColumnType("character varying(500)");

            builder.Property(c => c.DisplayOrder)
                .ValueGeneratedOnAdd()
                .HasColumnType("integer")
                .HasDefaultValue(0);           // ✅ 0 وليس 1

            builder.Property(c => c.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamp with time zone"); // ✅ nullable DateTime?

            builder.Property(c => c.DeletedAt)
                .HasColumnType("timestamp with time zone"); // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────
            builder.HasIndex(c => c.DisplayOrder)
                .HasDatabaseName("IX_Categories_DisplayOrder");

            // ──────────────────────────────────────────
            // Relationships
            // ──────────────────────────────────────────
            builder.HasMany(c => c.MenuItems)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ نمنع الحذف لو فيها منتجات

            // ──────────────────────────────────────────
            // Seed Data — نفس الـ Snapshot تماماً
            // ──────────────────────────────────────────
            builder.HasData(
                new
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Name = "المقبلات",
                    Description = "أشهى المقبلات العربية",
                    DisplayOrder = 1,
                    ImageUrl = "https://placehold.co/400x300?text=Appetizers",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Name = "الأطباق الرئيسية",
                    Description = "أطباق رئيسية شهية",
                    DisplayOrder = 2,
                    ImageUrl = "https://placehold.co/400x300?text=Main+Dishes",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    Name = "المشروبات",
                    Description = "مشروبات باردة وساخنة",
                    DisplayOrder = 3,
                    ImageUrl = "https://placehold.co/400x300?text=Drinks",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    Name = "الحلويات",
                    Description = "أحلى الحلويات الشرقية",
                    DisplayOrder = 4,
                    ImageUrl = "https://placehold.co/400x300?text=Desserts",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}

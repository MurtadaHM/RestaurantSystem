using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class TableConfiguration : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("Tables");

            // ──────────────────────────────────────────
            // Primary Key
            // ──────────────────────────────────────────
            builder.HasKey(t => t.Id);

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(t => t.TableNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("character varying(10)");

            builder.Property(t => t.Capacity)
                .IsRequired()
                .HasColumnType("integer");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()               // ✅ Enum → string
                .HasColumnType("text")
                .HasDefaultValue(TableStatus.Available);

            builder.Property(t => t.Location)
                .IsRequired(false)
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(t => t.Notes)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(t => t.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.UpdatedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            builder.Property(t => t.DeletedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────

            // ✅ TableNumber يجب أن يكون فريداً
            builder.HasIndex(t => t.TableNumber)
                .IsUnique()
                .HasDatabaseName("IX_Tables_TableNumber");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tables_Status");

            // ──────────────────────────────────────────
            // Seed Data
            // ──────────────────────────────────────────
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Table
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    TableNumber = "T1",
                    Capacity = 2,
                    Location = "بجانب النافذة",
                    Notes = "طاولة مريحة بإطلالة جميلة",
                    Status = TableStatus.Available,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    TableNumber = "T2",
                    Capacity = 4,
                    Location = "الوسط",
                    Notes = "طاولة مثالية للعائلات",
                    Status = TableStatus.Available,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    TableNumber = "T3",
                    Capacity = 6,
                    Location = "الزاوية",
                    Notes = "طاولة ممتازة للخصوصية",
                    Status = TableStatus.Available,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    TableNumber = "T4",
                    Capacity = 8,
                    Location = "قاعة VIP",
                    Notes = "طاولة فاخرة خاصة لكبار الزوار",
                    Status = TableStatus.Available,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );

            // ──────────────────────────────────────────
            // Soft Delete Filter
            // ──────────────────────────────────────────
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}

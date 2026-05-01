using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class TableConfiguration : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> builder)
        {
            builder.ToTable("Tables");

            // 1. Primary Key
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnType("uuid");

            // 2. Basic Properties
            builder.Property(t => t.TableNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("character varying(10)");

            // مهم للـ QR / Barcode / Public table lookup
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnType("character varying(30)");

            builder.Property(t => t.Capacity)
                .IsRequired()
                .HasColumnType("integer");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            builder.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(t => t.Zone)
                .HasMaxLength(50)
                .HasColumnType("character varying(50)");

            builder.Property(t => t.FloorNumber)
                .HasColumnType("integer");

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasColumnType("boolean");

            builder.Property(t => t.IsOrderingEnabled)
                .IsRequired()
                .HasColumnType("boolean");

            builder.Property(t => t.Notes)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            // 3. Timestamps / Soft Delete
            builder.Property(t => t.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // 4. Indexes
            builder.HasIndex(t => t.TableNumber)
     .IsUnique()
     .HasFilter("\"IsDeleted\" = false")
     .HasDatabaseName("IX_Tables_TableNumber");

            builder.HasIndex(t => t.Code)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false")
                .HasDatabaseName("IX_Tables_Code");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tables_Status");

            // 5. Seed Data
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Table
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    TableNumber = "T1",
                    Code = "TBL-T1",
                    Capacity = 2,
                    Location = "بجانب النافذة",
                    Zone = "الصالة الرئيسية",
                    FloorNumber = 1,
                    Notes = "طاولة مريحة بإطلالة جميلة",
                    Status = TableStatus.Available,
                    IsActive = true,
                    IsOrderingEnabled = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    TableNumber = "T2",
                    Code = "TBL-T2",
                    Capacity = 4,
                    Location = "الوسط",
                    Zone = "الصالة الرئيسية",
                    FloorNumber = 1,
                    Notes = "طاولة مثالية للعائلات",
                    Status = TableStatus.Available,
                    IsActive = true,
                    IsOrderingEnabled = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    TableNumber = "T3",
                    Code = "TBL-T3",
                    Capacity = 6,
                    Location = "الزاوية",
                    Zone = "الصالة الرئيسية",
                    FloorNumber = 1,
                    Notes = "طاولة ممتازة للخصوصية",
                    Status = TableStatus.Available,
                    IsActive = true,
                    IsOrderingEnabled = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Table
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    TableNumber = "T4",
                    Code = "TBL-T4",
                    Capacity = 8,
                    Location = "قاعة VIP",
                    Zone = "VIP",
                    FloorNumber = 1,
                    Notes = "طاولة فاخرة خاصة لكبار الزوار",
                    Status = TableStatus.Available,
                    IsActive = true,
                    IsOrderingEnabled = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );

            // 6. Soft Delete Filter
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
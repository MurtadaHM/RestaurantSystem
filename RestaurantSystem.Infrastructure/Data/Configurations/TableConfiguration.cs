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

            // 1. المفتاح الأساسي
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnType("uuid");

            // 2. الخصائص الأساسية
            builder.Property(t => t.TableNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("character varying(10)");

            builder.Property(t => t.Capacity)
                .IsRequired()
                .HasColumnType("integer");

            // ✅ التحويل لنص (text) بدون DefaultValue في الـ Fluent API لمنع الانهيار 🔥
            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            builder.Property(t => t.Location)
                .HasMaxLength(100);

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

            // 3. التواقيت (مهمة جداً لـ PostgreSQL لكي يقبل بيانات الـ Seed)
            builder.Property(t => t.IsDeleted).HasColumnType("boolean");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // 4. الفهارس
            builder.HasIndex(t => t.TableNumber).IsUnique().HasDatabaseName("IX_Tables_TableNumber");
            builder.HasIndex(t => t.Status).HasDatabaseName("IX_Tables_Status");

            // 5. Seed Data (البيانات الأولية)
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

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
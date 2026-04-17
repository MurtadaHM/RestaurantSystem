using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class DepartmentsConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd().HasColumnType("uuid");

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(d => d.Description)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(d => d.Icon)
                .HasMaxLength(50)
                .HasColumnType("character varying(50)");

            // ✅ الحل النهائي: التحويل لنص (text) مع إزالة القيمة الافتراضية من الـ Fluent API
            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            // خصائص الـ BaseEntity
            builder.Property(d => d.IsDeleted).HasColumnType("boolean");
            builder.Property(d => d.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(d => d.UpdatedAt).HasColumnType("timestamp with time zone");
            builder.Property(d => d.DeletedAt).HasColumnType("timestamp with time zone");

            builder.HasIndex(d => d.Name).IsUnique().HasDatabaseName("IX_Departments_Name");

            builder.HasMany(d => d.MenuItems)
                .WithOne(m => m.Department)
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
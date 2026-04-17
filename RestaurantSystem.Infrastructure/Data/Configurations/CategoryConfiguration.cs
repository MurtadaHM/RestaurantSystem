using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(c => c.DisplayOrder)
                .HasColumnType("integer")
                .HasDefaultValue(0);

            builder.Property(c => c.DepartmentId)
                .IsRequired()
                .HasColumnType("uuid");

            // خصائص التدقيق (Audit Properties)
            builder.Property(c => c.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // الفهارس
            builder.HasIndex(c => c.DisplayOrder)
                .HasDatabaseName("IX_Categories_DisplayOrder");

            // العلاقات
            builder.HasMany(c => c.MenuItems)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
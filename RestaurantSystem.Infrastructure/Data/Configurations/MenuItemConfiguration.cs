using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnType("uuid");

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)");

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            // 💰 ضبط السعر لـ PostgreSQL
            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            builder.Property(m => m.ImageUrl)
                .HasMaxLength(500);

            builder.Property(m => m.Ingredients)
                .HasMaxLength(1000);

            builder.Property(m => m.Calories)
                .HasColumnType("integer");

            // ✅ هنا DefaultValue مسموح لأن الحقل boolean وليس Enum
            builder.Property(m => m.IsAvailable)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            builder.Property(m => m.PreparationTimeMinutes)
                .HasColumnType("integer")
                .HasDefaultValue(15);

            // 🕒 التواقيت (مهمة جداً لنجاح الـ Seed)
            builder.Property(m => m.IsDeleted).HasColumnType("boolean");
            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(m => m.UpdatedAt).HasColumnType("timestamp with time zone");
            builder.Property(m => m.DeletedAt).HasColumnType("timestamp with time zone");

            // 🔍 الفهارس
            builder.HasIndex(m => m.CategoryId).HasDatabaseName("IX_MenuItems_CategoryId");
            builder.HasIndex(m => m.DepartmentId).HasDatabaseName("IX_MenuItems_DepartmentId");

            // 🤝 العلاقات (Relationships)

            // MenuItem → Category
            builder.HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 إضافة علاقة القسم (لأنك استخدمتها في الـ Seed)
            builder.HasOne(m => m.Department)
                .WithMany(d => d.MenuItems)
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}
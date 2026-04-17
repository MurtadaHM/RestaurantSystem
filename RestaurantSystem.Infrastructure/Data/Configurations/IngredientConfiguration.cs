using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            // 1. إعدادات الجدول
            builder.ToTable("Ingredients");

            // 2. المفتاح الأساسي
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasColumnType("uuid");

            // 3. الخصائص الأساسية
            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            // 🛠️ ضبط دقة الكمية (الوزن أو الحجم)
            builder.Property(i => i.CurrentStock)
                .HasPrecision(18, 3)
                .HasColumnType("numeric(18,3)");

            builder.Property(i => i.MinThreshold)
                .HasPrecision(18, 3)
                .HasColumnType("numeric(18,3)");

            // 💰 ضبط دقة السعر
            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)");

            // 🕒 تحديث خصائص الـ BaseEntity (مهم جداً لـ PostgreSQL)
            builder.Property(i => i.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(i => i.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone"); // ✅ التغيير هنا لضمان التوافق

            builder.Property(i => i.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(i => i.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // 4. العلاقات (Relationships)

            // علاقة المادة مع سجل الحركات (One-to-Many)
            builder.HasMany(i => i.StockMovements)
                .WithOne(sm => sm.Ingredient)
                .HasForeignKey(sm => sm.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة المادة مع الوصفات (MenuItemIngredients)
            builder.HasMany(i => i.MenuItemIngredients)
                .WithOne(mi => mi.Ingredient)
                .HasForeignKey(mi => mi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict); // 💡 نمنع حذف المادة إذا كانت مرتبطة بوصفة
        }
    }
}
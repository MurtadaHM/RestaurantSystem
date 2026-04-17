using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");

            // 1. المفتاح الأساسي
            builder.HasKey(sm => sm.Id);
            builder.Property(sm => sm.Id).HasColumnType("uuid");

            // 2. إعدادات الحقول
            builder.Property(sm => sm.Quantity)
                .IsRequired()
                .HasColumnType("numeric(18,3)"); // دقة عالية للموازين والأوزان

            builder.Property(sm => sm.Reason)
                .HasMaxLength(250)
                .HasColumnType("character varying(250)");

            // ✅ التحويل لنص (text) بدون DefaultValue لمنع خطأ الـ 500 🔥
            builder.Property(sm => sm.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            // 3. التواقيت (مهمة جداً لـ PostgreSQL)
            builder.Property(sm => sm.MovementDate)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(sm => sm.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(sm => sm.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(sm => sm.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(sm => sm.IsDeleted)
                .HasColumnType("boolean");

            // 4. العلاقات (Relationships)

            // الربط مع المادة الأولية
            builder.HasOne(sm => sm.Ingredient)
                .WithMany(i => i.StockMovements)
                .HasForeignKey(sm => sm.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // الربط مع المستخدم الذي قام بالحركة
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // 🔍 فهارس للبحث السريع
            builder.HasIndex(sm => sm.MovementDate);
            builder.HasIndex(sm => sm.IngredientId);
        }
    }
}
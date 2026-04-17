using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            // 1. المفتاح الأساسي
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnType("uuid");

            // 2. الخصائص الأساسية
            builder.Property(p => p.OrderId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("numeric(18,2)"); // 👈 النوع الأدق في Postgres للعملات

            // 3. التحويلات (Enums) -> تم حذف الـ DefaultValue لمنع خطأ EF8 🔥
            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");
            // ❌ تم حذف .HasDefaultValue(PaymentStatus.Pending) لمنع الانهيار

            // 4. الحقول النصية
            builder.Property(p => p.TransactionReference)
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(p => p.Notes)
                .HasMaxLength(500);

            // 5. التواقيت (مهمة جداً لنجاح الـ Seed)
            builder.Property(p => p.PaymentDate)
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.IsDeleted)
                .HasColumnType("boolean");

            // 6. الفهارس والعلاقات
            builder.HasIndex(p => p.OrderId)
                .IsUnique()
                .HasDatabaseName("IX_Payments_OrderId");

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
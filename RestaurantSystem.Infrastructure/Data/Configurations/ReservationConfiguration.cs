using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            // 1. المفتاح الأساسي
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnType("uuid");

            // 2. الخصائص الأساسية
            builder.Property(r => r.CustomerName)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("character varying(150)");

            builder.Property(r => r.CustomerPhone)
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            // ✅ الحل النهائي: تحويل الـ Enum لنص بدون DefaultValue في الـ Fluent API 🔥
            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");
            // ❌ تم حذف .HasDefaultValue لمنع خطأ EF8

            // 3. التواقيت
            // Keep other timestamps as timestamptz; store reservation time as local wall-clock time
            builder.Property(r => r.ReservationDate)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(r => r.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(r => r.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(r => r.IsDeleted)
                .HasColumnType("boolean");

            // 4. العلاقات (Relationships)
            builder.HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // فلتر الحذف الناعم
            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using System;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            // 1. المفتاح الأساسي
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnType("uuid");

            // 2. الحالات (Enums) -> تحويلها لنصوص لسهولة القراءة في DB
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            builder.Property(o => o.OrderType)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            builder.Property(o => o.ExternalDeliveryStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("text");

            // 3. الخصائص الأساسية
            builder.Property(o => o.OrderNumber)
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            builder.Property(o => o.DeliveryFee)
                .HasColumnType("numeric(18,2)");

            builder.Property(o => o.SpecialNotes)
                .HasMaxLength(500);

            // NEW: IsStockDeducted flag for inventory deduction protection
            builder.Property(o => o.IsStockDeducted)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            // 4. 🆕 حقول التكامل مع شركة التوصيل (Sendy) والإحداثيات
            builder.Property(o => o.ExternalOrderId)
                .HasColumnType("uuid");

            builder.Property(o => o.CustomerPhoneNumber)
                .HasMaxLength(20);

            builder.Property(o => o.DeliveryAddress)
                .HasMaxLength(1000);

            // 📍 الإحداثيات الجغرافية
            builder.Property(o => o.Latitude)
                .HasColumnType("double precision"); // Postgres type for double

            builder.Property(o => o.Longitude)
                .HasColumnType("double precision");

            // 👤 بيانات السائق
            builder.Property(o => o.CourierName)
                .HasMaxLength(200);

            builder.Property(o => o.CourierPhoneNumber)
                .HasMaxLength(20);

            builder.Property(o => o.IsSyncedToExternalProvider)
                .HasColumnType("boolean");

            // 5. التواقيت (Postgres timestamptz)
            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.ExpectedReadyTime)
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.CompletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(o => o.LastExternalSyncDate)
                .HasColumnType("timestamp with time zone");

            // 6. الفهارس (Indexes) - حاسمة للأداء
            builder.HasIndex(o => o.UserId).HasDatabaseName("IX_Orders_UserId");
            builder.HasIndex(o => o.Status).HasDatabaseName("IX_Orders_Status");
            builder.Property(o => o.CustomerId)
    .HasColumnType("uuid");

            builder.HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");


            // 🔥 فهرس المعرف الخارجي (مهم جداً لسرعة استجابة الـ Webhook)
            builder.HasIndex(o => o.ExternalOrderId)
                .HasDatabaseName("IX_Orders_ExternalOrderId");

            // 7. العلاقات
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(o => o.Customer)
    .WithMany(c => c.Orders)
    .HasForeignKey(o => o.CustomerId)
    .OnDelete(DeleteBehavior.SetNull);

            // فلتر الحذف الناعم
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
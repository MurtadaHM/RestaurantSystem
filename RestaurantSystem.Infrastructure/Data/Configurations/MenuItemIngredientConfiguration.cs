using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class MenuItemIngredientConfiguration : IEntityTypeConfiguration<MenuItemIngredient>
    {
        public void Configure(EntityTypeBuilder<MenuItemIngredient> builder)
        {
            builder.ToTable("MenuItemIngredients");

            // 1. 🛠️ ضبط المفتاح المركب (Composite Key)
            builder.HasKey(mi => new { mi.MenuItemId, mi.IngredientId });

            // 2. إعدادات الحقول
            builder.Property(mi => mi.MenuItemId).HasColumnType("uuid");
            builder.Property(mi => mi.IngredientId).HasColumnType("uuid");

            // تحديد دقة الكمية المطلوبة (مثلاً 0.250 كغم)
            builder.Property(mi => mi.Quantity)
                .IsRequired()
                .HasColumnType("numeric(18,3)"); // 👈 تحديد النوع لـ Postgres بدقة

            // 3. العلاقات (Relationships)

            // MenuItem ← MenuItemIngredients
            builder.HasOne(mi => mi.MenuItem)
                .WithMany(m => m.MenuItemIngredients)
                .HasForeignKey(mi => mi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade); // إذا انحذف الطبق، تنحذف مكوناته من الربط

            // Ingredient ← MenuItemIngredients
            builder.HasOne(mi => mi.Ingredient)
                .WithMany(i => i.MenuItemIngredients)
                .HasForeignKey(mi => mi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict); // نمنع حذف المادة الأساسية إذا كانت داخلة بوصفة
        }
    }
}
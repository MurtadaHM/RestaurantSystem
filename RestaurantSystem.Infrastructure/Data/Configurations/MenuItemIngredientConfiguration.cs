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

            // 1. Primary Key من BaseEntity
            builder.HasKey(mi => mi.Id);

            builder.Property(mi => mi.Id)
                .HasColumnType("uuid");

            // 2. منع تكرار نفس المادة داخل نفس الوصفة
            builder.HasIndex(mi => new { mi.MenuItemId, mi.IngredientId })
                .IsUnique()
                .HasDatabaseName("IX_MenuItemIngredients_MenuItemId_IngredientId");

            // 3. إعدادات الحقول الأساسية
            builder.Property(mi => mi.MenuItemId)
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(mi => mi.IngredientId)
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(mi => mi.Quantity)
                .IsRequired()
                .HasColumnType("numeric(18,3)");

            // 4. الحقول الجديدة للوصفة
            builder.Property(mi => mi.Notes)
                .HasMaxLength(500);

            builder.Property(mi => mi.IsOptional)
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            builder.Property(mi => mi.WastePercentage)
                .HasColumnType("numeric(5,2)")
                .HasDefaultValue(0);

            // 5. خصائص BaseEntity
            builder.Property(mi => mi.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(mi => mi.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(mi => mi.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(mi => mi.DeletedAt)
                .HasColumnType("timestamp with time zone");

            // 6. العلاقات
            builder.HasOne(mi => mi.MenuItem)
                .WithMany(m => m.MenuItemIngredients)
                .HasForeignKey(mi => mi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mi => mi.Ingredient)
                .WithMany(i => i.MenuItemIngredients)
                .HasForeignKey(mi => mi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
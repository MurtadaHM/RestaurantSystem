using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantSystem.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. تطبيق الإعدادات الفردية من الـ Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // 2. التأمين الشامل لجميع الـ Enums والتواريخ في النظام
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var type = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;

                    // تحويل الـ Enums إلى string
                    if (type.IsEnum)
                    {
                        var converterType = typeof(EnumToStringConverter<>).MakeGenericType(type);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;

                        property.SetValueConverter(converter);
                        property.Sentinel = Activator.CreateInstance(type);
                    }

                    // DateTime غير nullable
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }

                    // DateTime nullable
                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue
                                ? (v.Value.Kind == DateTimeKind.Utc
                                    ? v
                                    : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
                                : v,
                            v => v.HasValue
                                ? (v.Value.Kind == DateTimeKind.Utc
                                    ? v
                                    : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
                                : v));
                    }
                }

                // 3. فلاتر الحذف الناعم (Soft Delete)
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                            Expression.Constant(false)),
                        parameter
                    );

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                }
            }
        }

        // كلاس مساعد للتحويل
        private class SymbolToStringConverter : ValueConverter<object, string>
        {
            public SymbolToStringConverter(Type type)
                : base(v => v.ToString()!, v => Enum.Parse(type, v)) { }
        }
    }
}
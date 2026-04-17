using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // 1. تنفيذ المايجريشن لضمان وجود الجداول
            await context.Database.MigrateAsync();

            // 2. إضافة المستخدمين لكل الأدوار (Seed All Roles) 🔥
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new() {
                        Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                        FirstName = "Murtada", LastName = "Admin",
                        Email = "admin@restaurant.com", Role = UserRole.Admin,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Ahmed", LastName = "Manager",
                        Email = "manager@restaurant.com", Role = UserRole.Manager,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Sami", LastName = "Chef",
                        Email = "chef@restaurant.com", Role = UserRole.Chef,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Chef@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Zaid", LastName = "Waiter",
                        Email = "waiter@restaurant.com", Role = UserRole.Waiter,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Waiter@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Noor", LastName = "Cashier",
                        Email = "cashier@restaurant.com", Role = UserRole.Cashier,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cashier@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Ali", LastName = "Driver",
                        Email = "driver@restaurant.com", Role = UserRole.DeliveryDriver,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        FirstName = "Jassem", LastName = "Customer",
                        Email = "customer@restaurant.com", Role = UserRole.Customer,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ All system users (Roles) have been created.");
            }

            // 3. إضافة باقي البيانات (Departments, Categories, etc.)
            // سيكمل الكود هنا إذا لم يكن هناك أخطاء في الـ Configurations
            try
            {
                if (!await context.Departments.AnyAsync())
                {
                    // كود الأقسام الخاص بك...
                }
                // ... باقي بيانات الـ Seed
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Data seeding partially failed, but users are safe. Error: {ex.Message}");
            }
        }
    }
}
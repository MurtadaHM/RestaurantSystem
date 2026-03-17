using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ✅ استبدلنا مكتبة Identity بمكتبة BCrypt
using BCrypt.Net;

namespace RestaurantSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            // 1. إضافة الفئات (Categories)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Id = Guid.NewGuid(), Name = "المشويات", DisplayOrder = 1, Description = "أشهى أنواع اللحوم المشوية" },
                    new() { Id = Guid.NewGuid(), Name = "المقبلات", DisplayOrder = 2, Description = "مقبلات باردة وحارة" },
                    new() { Id = Guid.NewGuid(), Name = "المشروبات", DisplayOrder = 3, Description = "عصائر ومشروبات غازية" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 2. إضافة الطاولات (Tables)
            if (!await context.Tables.AnyAsync())
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.Tables.Add(new Table
                    {
                        Id = Guid.NewGuid(),
                        TableNumber = i.ToString(),
                        Capacity = (i % 2 == 0) ? 4 : 2,
                        Status = TableStatus.Available
                    });
                }
            }

            // 3. إضافة مستخدم Admin (تم التعديل لاستخدام BCrypt)
            if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                var admin = new User
                {
                    // استخدمنا الـ ID الثابت لضمان عدم التكرار أثناء الاختبار
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    FirstName = "Murtada",
                    LastName = "Admin",
                    Email = "admin@restaurant.com",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // ✅ التشفير باستخدام BCrypt ليطابق الـ AuthService
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                await context.Users.AddAsync(admin);
            }

            await context.SaveChangesAsync();
        }
    }
}
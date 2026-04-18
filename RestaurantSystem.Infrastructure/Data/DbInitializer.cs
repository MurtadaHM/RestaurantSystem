using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace RestaurantSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            try
            {
                await SeedUsersAsync(context);
                await SeedOtherDataAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Data seeding failed. Error: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var usersToAdd = new List<User>();

            if (!await context.Users.AnyAsync(u => u.Email == "admin@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    FirstName = "Murtada",
                    LastName = "Admin",
                    Email = "admin@restaurant.com",
                    Role = UserRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "manager@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ahmed",
                    LastName = "Manager",
                    Email = "manager@restaurant.com",
                    Role = UserRole.Manager,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "chef@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Sami",
                    LastName = "Chef",
                    Email = "chef@restaurant.com",
                    Role = UserRole.Chef,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Chef@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "waiter@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Zaid",
                    LastName = "Waiter",
                    Email = "waiter@restaurant.com",
                    Role = UserRole.Waiter,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Waiter@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "cashier@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Noor",
                    LastName = "Cashier",
                    Email = "cashier@restaurant.com",
                    Role = UserRole.Cashier,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cashier@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "driver@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ali",
                    LastName = "Driver",
                    Email = "driver@restaurant.com",
                    Role = UserRole.DeliveryDriver,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Driver@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await context.Users.AnyAsync(u => u.Email == "customer@restaurant.com"))
            {
                usersToAdd.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Jassem",
                    LastName = "Customer",
                    Email = "customer@restaurant.com",
                    Role = UserRole.Customer,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (usersToAdd.Count > 0)
            {
                await context.Users.AddRangeAsync(usersToAdd);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Seeded missing system users successfully.");
            }
            else
            {
                Console.WriteLine("ℹ️ System users already exist. Skipping user seeding.");
            }
        }

        private static async Task SeedOtherDataAsync(ApplicationDbContext context)
        {
            try
            {
                if (!await context.Departments.AnyAsync())
                {
                    // كود الأقسام هنا
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Other seed data partially failed, but users are safe. Error: {ex.Message}");
            }
        }
    }
}
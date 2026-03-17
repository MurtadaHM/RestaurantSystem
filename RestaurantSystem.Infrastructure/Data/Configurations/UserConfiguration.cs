using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // ──────────────────────────────────────────
            // Table
            // ──────────────────────────────────────────
            builder.ToTable("Users");

            // ──────────────────────────────────────────
            // Primary Key
            // ──────────────────────────────────────────
            builder.HasKey(u => u.Id);

            // ──────────────────────────────────────────
            // Properties
            // ──────────────────────────────────────────
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("character varying(255)");

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(u => u.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            builder.Property(u => u.ProfileImageUrl)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(u => u.Address)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            builder.Property(u => u.City)
                .IsRequired(false)
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()          // ✅ Enum → string
                .HasColumnType("text")
                .HasDefaultValue(UserRole.Customer);

            builder.Property(u => u.IsActive)
                .HasColumnType("boolean")
                .HasDefaultValue(true);

            builder.Property(u => u.LastLoginAt)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(u => u.IsDeleted)
                .HasColumnType("boolean");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            builder.Property(u => u.DeletedAt)
                .HasColumnType("timestamp with time zone");  // ✅ nullable

            // ──────────────────────────────────────────
            // Indexes
            // ──────────────────────────────────────────

            // ✅ Email فريد
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // ──────────────────────────────────────────
            // Seed Data — Admin User
            // ──────────────────────────────────────────
            builder.HasData(new User
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@restaurant.com",
                PasswordHash = "$2a$11$Xn3XqJ8Zv5kL2mN7pR9sOeKjHgFdCbA4wY6uT1iE0oP3qS5vU8yW",
                PhoneNumber = "0500000000",
                Role = UserRole.Admin,
                IsActive = true,
                IsDeleted = false,
                Address = null,
                City = null,
                ProfileImageUrl = null,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // ──────────────────────────────────────────
            // Soft Delete Filter
            // ──────────────────────────────────────────
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}

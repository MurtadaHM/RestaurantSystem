using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnType("uuid");

            builder.Property(c => c.FullName)
                .HasMaxLength(150);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            builder.HasIndex(c => c.PhoneNumber)
                .HasDatabaseName("IX_Customers_PhoneNumber");

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Data.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.UserId)
                .HasColumnType("uuid");

            builder.Property(x => x.UserName)
                .HasMaxLength(150);

            builder.Property(x => x.UserRole)
                .HasMaxLength(100);

            builder.Property(x => x.ActionType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.EntityName)
                .HasMaxLength(150);

            builder.Property(x => x.EntityId)
                .HasColumnType("uuid");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.OldValue)
                .HasColumnType("text");

            builder.Property(x => x.NewValue)
                .HasColumnType("text");

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.Timestamp)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasColumnType("boolean")
                .HasDefaultValue(false);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.Timestamp)
                .HasDatabaseName("IX_ActivityLogs_Timestamp");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_ActivityLogs_UserId");

            builder.HasIndex(x => x.Module)
                .HasDatabaseName("IX_ActivityLogs_Module");

            builder.HasIndex(x => x.ActionType)
                .HasDatabaseName("IX_ActivityLogs_ActionType");

            builder.HasIndex(x => new { x.Timestamp, x.Module })
                .HasDatabaseName("IX_ActivityLogs_Timestamp_Module");

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
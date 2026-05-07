using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IActivityLogRepository
    {
        Task AddAsync(ActivityLog activityLog);

        Task<ActivityLog?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<ActivityLog>> GetAsync(ActivityLogFilterDto filter);

        Task<int> CountAsync(ActivityLogFilterDto filter);

        Task SaveChangesAsync();
    }
}
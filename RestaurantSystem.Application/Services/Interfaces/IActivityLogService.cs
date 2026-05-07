using RestaurantSystem.Application.DTOs.ActivityLogs;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(CreateActivityLogDto dto);

        Task<ActivityLogResponseDto?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<ActivityLogResponseDto>> GetAsync(ActivityLogFilterDto filter);

        Task<int> CountAsync(ActivityLogFilterDto filter);
    }
}
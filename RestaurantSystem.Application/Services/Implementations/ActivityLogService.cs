using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        public async Task LogAsync(CreateActivityLogDto dto)
        {
            if (dto == null)
                return;

            if (string.IsNullOrWhiteSpace(dto.Module))
                return;

            if (string.IsNullOrWhiteSpace(dto.Description))
                return;

            var activityLog = new ActivityLog
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserRole = dto.UserRole,
                ActionType = dto.ActionType,
                Module = dto.Module.Trim(),
                EntityName = string.IsNullOrWhiteSpace(dto.EntityName)
                    ? null
                    : dto.EntityName.Trim(),
                EntityId = dto.EntityId,
                Description = dto.Description.Trim(),
                OldValue = dto.OldValue,
                NewValue = dto.NewValue,
                IpAddress = dto.IpAddress,
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _activityLogRepository.AddAsync(activityLog);
            await _activityLogRepository.SaveChangesAsync();
        }

        public async Task<ActivityLogResponseDto?> GetByIdAsync(Guid id)
        {
            var log = await _activityLogRepository.GetByIdAsync(id);

            if (log == null)
                return null;

            return MapToResponseDto(log);
        }

        public async Task<IReadOnlyList<ActivityLogResponseDto>> GetAsync(ActivityLogFilterDto filter)
        {
            filter ??= new ActivityLogFilterDto();

            NormalizePaging(filter);

            var logs = await _activityLogRepository.GetAsync(filter);

            return logs
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<int> CountAsync(ActivityLogFilterDto filter)
        {
            filter ??= new ActivityLogFilterDto();

            return await _activityLogRepository.CountAsync(filter);
        }

        private static void NormalizePaging(ActivityLogFilterDto filter)
        {
            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = 50;

            if (filter.PageSize > 200)
                filter.PageSize = 200;
        }

        private static ActivityLogResponseDto MapToResponseDto(ActivityLog log)
        {
            return new ActivityLogResponseDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                UserId = log.UserId,
                UserName = log.UserName,
                UserRole = log.UserRole,
                ActionType = log.ActionType,
                Module = log.Module,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Description = log.Description,
                OldValue = log.OldValue,
                NewValue = log.NewValue,
                IpAddress = log.IpAddress
            };
        }
    }
}
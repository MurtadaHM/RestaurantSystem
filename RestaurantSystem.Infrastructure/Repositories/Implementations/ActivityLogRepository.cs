using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ActivityLog activityLog)
        {
            await _context.ActivityLogs.AddAsync(activityLog);
        }

        public async Task<ActivityLog?> GetByIdAsync(Guid id)
        {
            return await _context.ActivityLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<ActivityLog>> GetAsync(ActivityLogFilterDto filter)
        {
            var query = BuildQuery(filter);

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 50 : filter.PageSize;

            if (pageSize > 200)
                pageSize = 200;

            return await query
                .OrderByDescending(x => x.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountAsync(ActivityLogFilterDto filter)
        {
            var query = BuildQuery(filter);

            return await query.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<ActivityLog> BuildQuery(ActivityLogFilterDto filter)
        {
            filter ??= new ActivityLogFilterDto();

            var query = _context.ActivityLogs.AsQueryable();

            if (filter.From.HasValue)
            {
                var from = EnsureUtc(filter.From.Value);
                query = query.Where(x => x.Timestamp >= from);
            }

            if (filter.To.HasValue)
            {
                var to = EnsureUtc(filter.To.Value);
                query = query.Where(x => x.Timestamp <= to);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == filter.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                var userName = filter.UserName.Trim().ToLower();

                query = query.Where(x =>
                    x.UserName != null &&
                    x.UserName.ToLower().Contains(userName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Module))
            {
                var module = filter.Module.Trim().ToLower();

                query = query.Where(x =>
                    x.Module.ToLower() == module);
            }

            if (filter.ActionType.HasValue)
            {
                query = query.Where(x => x.ActionType == filter.ActionType.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Description.ToLower().Contains(search) ||
                    (x.UserName != null && x.UserName.ToLower().Contains(search)) ||
                    (x.UserRole != null && x.UserRole.ToLower().Contains(search)) ||
                    x.Module.ToLower().Contains(search) ||
                    (x.EntityName != null && x.EntityName.ToLower().Contains(search)));
            }

            return query;
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;

            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
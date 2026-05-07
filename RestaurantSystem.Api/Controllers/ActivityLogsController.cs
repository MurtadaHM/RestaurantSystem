using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/activity-logs")]
    [Produces("application/json")]
    [Tags("Activity Logs")]
    [Authorize(Roles = "Admin,Manager")]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetActivityLogs(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] Guid? userId,
            [FromQuery] string? userName,
            [FromQuery] string? module,
            [FromQuery] ActivityActionType? actionType,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var filter = new ActivityLogFilterDto
            {
                From = from,
                To = to,
                UserId = userId,
                UserName = userName,
                Module = module,
                ActionType = actionType,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var logs = await _activityLogService.GetAsync(filter);
            var totalCount = await _activityLogService.CountAsync(filter);

            var response = new
            {
                items = logs,
                totalCount,
                pageNumber = filter.PageNumber,
                pageSize = filter.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };

            return Ok(ApiResponse<object>.Ok(response));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ActivityLogResponseDto>>> GetActivityLogById(Guid id)
        {
            var log = await _activityLogService.GetByIdAsync(id);

            if (log == null)
                return NotFound(ApiResponse<ActivityLogResponseDto>.Fail("Activity log not found."));

            return Ok(ApiResponse<ActivityLogResponseDto>.Ok(log));
        }
    }
}
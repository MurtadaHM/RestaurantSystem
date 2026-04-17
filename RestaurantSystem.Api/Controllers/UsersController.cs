using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Users Management")]
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserManagementService userManagementService,
            ILogger<UsersController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// إنشاء موظف جديد
        /// </summary>
        [HttpPost("staff")]
        public async Task<ActionResult<ApiResponse<UserListItemDto>>> CreateStaff([FromBody] CreateStaffRequestDto request)
        {
            _logger.LogInformation("محاولة إنشاء موظف جديد: {Email} بدور {Role}", request.Email, request.Role);

            var user = await _userManagementService.CreateStaffAsync(request);

            return Ok(ApiResponse<UserListItemDto>.Ok(user, "تم إنشاء الموظف بنجاح"));
        }

        /// <summary>
        /// جلب جميع المستخدمين
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserListItemDto>>>> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserListItemDto>>.Ok(users, "تم جلب المستخدمين بنجاح"));
        }

        /// <summary>
        /// جلب المستخدمين حسب الدور
        /// </summary>
        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserListItemDto>>>> GetUsersByRole(string role)
        {
            var users = await _userManagementService.GetUsersByRoleAsync(role);
            return Ok(ApiResponse<IEnumerable<UserListItemDto>>.Ok(users, "تم جلب المستخدمين حسب الدور بنجاح"));
        }

        /// <summary>
        /// تحديث دور مستخدم
        /// </summary>
        [HttpPatch("{userId:guid}/role")]
        public async Task<ActionResult<ApiResponse<UserListItemDto>>> UpdateUserRole(
            Guid userId,
            [FromBody] UpdateUserRoleRequestDto request)
        {
            _logger.LogInformation("محاولة تحديث دور المستخدم {UserId} إلى {Role}", userId, request.Role);

            var updatedUser = await _userManagementService.UpdateUserRoleAsync(userId, request);

            return Ok(ApiResponse<UserListItemDto>.Ok(updatedUser, "تم تحديث دور المستخدم بنجاح"));
        }

        /// <summary>
        /// تفعيل / تعطيل مستخدم
        /// </summary>
        [HttpPatch("{userId:guid}/status")]
        public async Task<ActionResult<ApiResponse<object>>> ToggleUserStatus(
            Guid userId,
            [FromBody] ToggleUserStatusRequestDto request)
        {
            _logger.LogInformation("محاولة تحديث حالة المستخدم {UserId} إلى {Status}", userId, request.IsActive);

            await _userManagementService.ToggleUserStatusAsync(userId, request);

            var message = request.IsActive
                ? "تم تفعيل المستخدم بنجاح"
                : "تم تعطيل المستخدم بنجاح";

            return Ok(ApiResponse<object>.Ok(null, message));
        }
    }
}
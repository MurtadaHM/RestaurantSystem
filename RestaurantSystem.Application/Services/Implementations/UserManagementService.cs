using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IActivityLogService _activityLogService;

        public UserManagementService(
            IUserRepository userRepository,
            IActivityLogService activityLogService)
        {
            _userRepository = userRepository;
            _activityLogService = activityLogService;
        }

        public async Task<UserListItemDto> CreateStaffAsync(CreateStaffRequestDto request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new Exception("البريد الإلكتروني مستخدم بالفعل");

            if (await _userRepository.ExistsByPhoneAsync(request.PhoneNumber))
                throw new Exception("رقم الهاتف مستخدم بالفعل");

            if (request.Password != request.ConfirmPassword)
                throw new Exception("كلمة المرور وتأكيدها غير متطابقين");

            if (request.Role == UserRole.Customer)
                throw new Exception("لا يمكن إنشاء موظف بدور Customer");

            var user = new User
            {
                Email = request.Email.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = user.Id,
                UserName = BuildUserName(user),
                UserRole = user.Role.ToString(),
                ActionType = ActivityActionType.UserCreated,
                Module = "Users",
                EntityName = nameof(User),
                EntityId = user.Id,
                Description = $"Created staff user '{BuildUserName(user)}' with role {user.Role}.",
                NewValue = BuildUserValue(user)
            });

            return MapToUserListItemDto(user);
        }

        public async Task<IEnumerable<UserListItemDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToUserListItemDto);
        }

        public async Task<IEnumerable<UserListItemDto>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new Exception("الدور مطلوب");

            if (!Enum.TryParse<UserRole>(role.Trim(), true, out var parsedRole))
                throw new Exception("الدور غير صحيح");

            var users = await _userRepository.GetByRoleAsync(parsedRole);
            return users.Select(MapToUserListItemDto);
        }

        public async Task<UserListItemDto> UpdateUserRoleAsync(Guid userId, UpdateUserRoleRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new Exception("المستخدم غير موجود");

            if (request.Role == UserRole.Customer)
                throw new Exception("لا يمكن تعيين المستخدم كموظف بدور Customer");

            var oldRole = user.Role;

            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = user.Id,
                UserName = BuildUserName(user),
                UserRole = user.Role.ToString(),
                ActionType = ActivityActionType.UserRoleChanged,
                Module = "Users",
                EntityName = nameof(User),
                EntityId = user.Id,
                Description = $"Changed user '{BuildUserName(user)}' role from {oldRole} to {user.Role}.",
                OldValue = oldRole.ToString(),
                NewValue = user.Role.ToString()
            });

            return MapToUserListItemDto(user);
        }

        public async Task<bool> ToggleUserStatusAsync(Guid userId, ToggleUserStatusRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new Exception("المستخدم غير موجود");

            var oldStatus = user.IsActive;

            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = user.Id,
                UserName = BuildUserName(user),
                UserRole = user.Role.ToString(),
                ActionType = ActivityActionType.UserStatusChanged,
                Module = "Users",
                EntityName = nameof(User),
                EntityId = user.Id,
                Description = user.IsActive
                    ? $"Activated user '{BuildUserName(user)}'."
                    : $"Deactivated user '{BuildUserName(user)}'.",
                OldValue = oldStatus ? "Active" : "Inactive",
                NewValue = user.IsActive ? "Active" : "Inactive"
            });

            return true;
        }

        private static UserListItemDto MapToUserListItemDto(User user)
        {
            return new UserListItemDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        private async Task SafeLogActivityAsync(CreateActivityLogDto dto)
        {
            try
            {
                await _activityLogService.LogAsync(dto);
            }
            catch
            {
                // Activity logging should never break user management operations.
            }
        }

        private static string BuildUserName(User user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            return string.IsNullOrWhiteSpace(fullName)
                ? user.Email
                : fullName;
        }

        private static string BuildUserValue(User user)
        {
            return
                $"FullName={BuildUserName(user)}; " +
                $"Email={user.Email}; " +
                $"Phone={user.PhoneNumber}; " +
                $"Role={user.Role}; " +
                $"IsActive={user.IsActive}; " +
                $"Address={user.Address}; " +
                $"City={user.City}";
        }
    }
}
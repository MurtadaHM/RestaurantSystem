using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return MapToUserListItemDto(user);
        }

        public async Task<bool> ToggleUserStatusAsync(Guid userId, ToggleUserStatusRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new Exception("المستخدم غير موجود");

            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
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
    }
}
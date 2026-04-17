using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        // البحث والتحقق الأساسي
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneAsync(string phoneNumber);

        // إدارة المستخدمين
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<User?> GetByIdAsync(Guid id);

        // تحديثات الإدارة
        Task UpdateRoleAsync(Guid userId, UserRole role);
        Task ToggleStatusAsync(Guid userId, bool isActive);

    }
}
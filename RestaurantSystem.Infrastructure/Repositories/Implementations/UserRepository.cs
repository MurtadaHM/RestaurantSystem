using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        // جلب المستخدم عن طريق الإيميل
        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        // التحقق إذا الإيميل موجود مسبقاً
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        // التحقق إذا رقم الهاتف موجود مسبقاً
        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            var normalizedPhone = phoneNumber.Trim();

            return await _dbSet
                .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber == normalizedPhone);
        }

        // جلب كل المستخدمين
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // جلب المستخدمين حسب الدور
        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.Role == role && !u.IsDeleted)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        // جلب مستخدم محدد بالـ id
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        // تحديث دور المستخدم
        public async Task UpdateRoleAsync(Guid userId, UserRole role)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user is not null)
            {
                user.Role = role;
                user.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(user);
            }
        }

        // تفعيل / تعطيل المستخدم
        public async Task ToggleStatusAsync(Guid userId, bool isActive)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user is not null)
            {
                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(user);
            }
        }
    }
}
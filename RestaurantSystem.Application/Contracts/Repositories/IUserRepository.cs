using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        // ✅ User? بدل User لتجنب مشاكل الـ Null
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
    }
}

using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    // ✅ التصحيح هنا: أضفنا الوراثة من الكلاس الأساسي 'Repository<Department>'
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// جلب الأقسام الفعالة فقط
        /// </summary>
        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => d.Status == DepartmentStatus.Active)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// جلب القسم مع قائمة أصناف المنيو (ضروري لشرط الحذف)
        /// </summary>
        public async Task<Department?> GetWithMenuItemsAsync(Guid id)
        {
            return await _context.Departments
                .Include(d => d.MenuItems)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
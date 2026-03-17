using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    // أضفنا where T : class لضمان أن هذا الانترفيس يتعامل مع كلاسات (جداول) فقط
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        // دالة للبحث المخصص (مثال: جلب المنتجات اللي سعرها أكبر من 50)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);
    }
}

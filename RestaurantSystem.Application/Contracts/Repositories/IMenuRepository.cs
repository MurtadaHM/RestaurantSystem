using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    /// <summary>
    /// مستودع إدارة أصناف المنيو (الأطباق، المشروبات، إلخ)
    /// </summary>
    public interface IMenuRepository : IRepository<MenuItem>
    {
        // ✅ جلب الأصناف حسب الفئة
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(Guid categoryId);

        // 🆕 جلب الأصناف حسب القسم التشغيلي (مطبخ، بارستا..)
        Task<IEnumerable<MenuItem>> GetByDepartmentAsync(Guid departmentId);

        // ✅ جلب الأصناف المتوفرة فقط
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();

        // ✅ البحث الذكي بالاسم
        Task<IEnumerable<MenuItem>> SearchByNameAsync(string name);

        // ✅ جلب الأصناف في نطاق سعري محدد
        Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // ✅ جلب الأصناف الأكثر طلباً
        Task<IEnumerable<MenuItem>> GetMostOrderedAsync(int topCount = 10);

        // ✅ تحديث حالة التوفر بسرعة
        Task UpdateAvailabilityAsync(Guid menuItemId, bool isAvailable);

        // 🔥 السطر السحري: إضافة تعريف الحفظ في قاعدة البيانات لحل خطأ الـ Build
        Task SaveChangesAsync();
    }
}
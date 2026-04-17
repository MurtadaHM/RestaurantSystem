using RestaurantSystem.Application.DTOs.Departments;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Interfaces
{
    /// <summary>
    /// واجهة التعامل مع الأقسام التشغيلية (مطبخ، بار، إلخ)
    /// </summary>
    public interface IDepartmentService
    {
        // ──────────────────────────────────────────
        // عمليات القراءة (Query Operations)
        // ──────────────────────────────────────────

        /// <summary>جلب كل الأقسام الموجودة في النظام</summary>
        Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync();

        /// <summary>جلب الأقسام الفعالة فقط (تُستخدم في شاشات الطلب)</summary>
        Task<IEnumerable<DepartmentResponseDto>> GetActiveDepartmentsAsync();

        /// <summary>جلب تفاصيل قسم محدد بالـ ID</summary>
        Task<DepartmentResponseDto> GetDepartmentByIdAsync(Guid id);

        // ──────────────────────────────────────────
        // عمليات الكتابة (Command Operations)
        // ──────────────────────────────────────────

        /// <summary>إنشاء قسم جديد</summary>
        Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentRequestDto request);

        /// <summary>تحديث بيانات قسم موجود</summary>
        Task<DepartmentResponseDto> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequestDto request);

        /// <summary>تحديث حالة القسم فقط (Active, Busy, Inactive)</summary>
        Task<bool> UpdateStatusAsync(Guid id, DepartmentStatus status);

        /// <summary>حذف قسم (مع التأكد من عدم وجود أصناف مرتبطة به)</summary>
        Task<bool> DeleteDepartmentAsync(Guid id);
    }
}
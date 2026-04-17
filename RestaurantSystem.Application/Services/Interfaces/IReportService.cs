using RestaurantSystem.Application.DTOs.Reports;

namespace RestaurantSystem.Application.Services.Interfaces
{
    /// <summary>
    /// واجهة استخراج التقارير والبيانات التحليلية للمطعم
    /// </summary>
    public interface IReportService
    {
        /// <summary>جلب ملخص عام للداشبورد (إجمالي الإيرادات، الطلبات، والزبائن)</summary>
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);

        /// <summary>جلب قائمة الأصناف الأكثر مبيعاً</summary>
        Task<IEnumerable<TopMenuItemDto>> GetTopSellingItemsAsync(int count);

        /// <summary>جلب تحليل المبيعات حسب فئة الصنف (مقبلات، مشويات، إلخ)</summary>
        Task<IEnumerable<CategorySalesDto>> GetSalesByCategoryAsync();

        // ──────────────────────────────────────────
        // 🚀 الإضافة الجديدة: تحليل المبيعات حسب القسم
        // ──────────────────────────────────────────
        /// <summary>جلب تحليل المبيعات لكل قسم تشغيلي (مطبخ، بار، حلويات)</summary>
        Task<IEnumerable<DepartmentSalesDto>> GetSalesByDepartmentAsync();
    }
}
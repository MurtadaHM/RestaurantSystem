
using RestaurantSystem.Application.DTOs.Reports;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TopMenuItemDto>> GetTopSellingItemsAsync(int count);
        Task<IEnumerable<CategorySalesDto>> GetSalesByCategoryAsync();
    }
}

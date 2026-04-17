using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common; 
using RestaurantSystem.Application.DTOs.Reports;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")] // التقارير حساسة، للمدراء فقط 🔐
    [ApiController]
    [Route("api/v1/[controller]")]
    [Tags("Reports & Analytics")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>جلب ملخص عام للأرقام (Revenue, Orders, Customers)</summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // إذا لم يتم إرسال تاريخ، نأخذ مبيعات اليوم افتراضياً
            startDate = startDate == default ? DateTime.UtcNow.Date : startDate;
            endDate = endDate == default ? DateTime.UtcNow : endDate;

            var result = await _reportService.GetDashboardSummaryAsync(startDate, endDate);
            return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
        }

        /// <summary>جلب أكثر الوجبات مبيعاً</summary>
        [HttpGet("top-items")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TopMenuItemDto>>>> GetTopItems([FromQuery] int count = 5)
        {
            var result = await _reportService.GetTopSellingItemsAsync(count);
            return Ok(ApiResponse<IEnumerable<TopMenuItemDto>>.Ok(result));
        }

        /// <summary>توزيع المبيعات حسب الفئة (مثلاً: مشروبات vs مشويات)</summary>
        [HttpGet("by-category")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategorySalesDto>>>> GetByCategory()
        {
            var result = await _reportService.GetSalesByCategoryAsync();
            return Ok(ApiResponse<IEnumerable<CategorySalesDto>>.Ok(result));
        }

        /// <summary>توزيع المبيعات حسب الأقسام التشغيلية (مطبخ، بار، حلويات)</summary>
        [HttpGet("by-department")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentSalesDto>>>> GetByDepartment()
        {
            var result = await _reportService.GetSalesByDepartmentAsync();
            return Ok(ApiResponse<IEnumerable<DepartmentSalesDto>>.Ok(result));
        }
    }
}
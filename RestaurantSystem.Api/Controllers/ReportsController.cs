using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Reports;
using RestaurantSystem.Application.Services.Interfaces;


[Authorize(Roles = "Admin,Manager")] // التقارير للمدراء فقط
[ApiController]
[Route("api/v1/[controller]")]
[Tags("Reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var startDate = start ?? DateTime.UtcNow.AddDays(-30); // الافتراضي آخر شهر
        var endDate = end ?? DateTime.UtcNow;

        var result = await _reportService.GetDashboardSummaryAsync(startDate, endDate);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
    }

    [HttpGet("top-items")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TopMenuItemDto>>>> GetTopItems([FromQuery] int count = 5)
    {
        var result = await _reportService.GetTopSellingItemsAsync(count);
        return Ok(ApiResponse<IEnumerable<TopMenuItemDto>>.Ok(result));
    }

    [HttpGet("sales-by-category")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategorySalesDto>>>> GetSalesByCategory()
    {
        var result = await _reportService.GetSalesByCategoryAsync();
        return Ok(ApiResponse<IEnumerable<CategorySalesDto>>.Ok(result));
    }
}
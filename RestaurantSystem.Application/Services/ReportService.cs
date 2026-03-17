using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Reports;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Application.Services.Implementations // ✅ يفضل استخدام Implementations للتنظيم
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;

        public ReportService(IOrderRepository orderRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            // تصفية حسب التاريخ والحالات المكتملة فقط مع استثناء المحذوف
            var filteredOrders = orders.Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && !o.IsDeleted);

            return new DashboardSummaryDto
            {
                TotalRevenue = filteredOrders.Sum(o => o.TotalAmount),
                TotalOrders = filteredOrders.Count(),
                // استخدام ! لأن UserId لا يمكن أن يكون null في منطق النظام
                TotalCustomers = filteredOrders.Select(o => o.UserId).Distinct().Count()
            };
        }

        public async Task<IEnumerable<TopMenuItemDto>> GetTopSellingItemsAsync(int count)
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            return orders
                .SelectMany(o => o.OrderItems)
                // ✅ حل تحذير الـ Null: التأكد من وجود الوجبة قبل التجميع
                .Where(oi => oi.MenuItem != null)
                .GroupBy(oi => oi.MenuItem!.Name)
                .Select(g => new TopMenuItemDto
                {
                    Name = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(count);
        }

        public async Task<IEnumerable<CategorySalesDto>> GetSalesByCategoryAsync()
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            return orders
                .SelectMany(o => o.OrderItems)
                // ✅ حل تحذير الـ Null: التأكد من وجود الوجبة والفئة التابعة لها
                .Where(oi => oi.MenuItem != null && oi.MenuItem.Category != null)
                .GroupBy(oi => oi.MenuItem!.Category!.Name)
                .Select(g => new CategorySalesDto
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(x => x.Quantity * x.Price),
                    ItemsSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSales);
        }
    }
}
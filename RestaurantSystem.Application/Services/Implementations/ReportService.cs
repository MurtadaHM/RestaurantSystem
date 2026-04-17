using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Reports;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;

        public ReportService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            var filteredOrders = orders.Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && !o.IsDeleted);

            return new DashboardSummaryDto
            {
                TotalRevenue = filteredOrders.Sum(o => o.TotalAmount),
                TotalOrders = filteredOrders.Count(),
                TotalCustomers = filteredOrders.Select(o => o.UserId).Distinct().Count()
            };
        }

        public async Task<IEnumerable<DepartmentSalesDto>> GetSalesByDepartmentAsync()
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            return orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.MenuItem != null && oi.MenuItem.Department != null)
                .GroupBy(oi => oi.MenuItem!.Department!.Name)
                .Select(g => new DepartmentSalesDto
                {
                    DepartmentName = g.Key,
                    TotalSales = g.Sum(x => x.Quantity * x.Price),
                    ItemsSold = g.Sum(x => x.Quantity) // ✅ تم تصحيح الاسم هنا من ItemsCount إلى ItemsSold
                })
                .OrderByDescending(x => x.TotalSales);
        }

        public async Task<IEnumerable<TopMenuItemDto>> GetTopSellingItemsAsync(int count)
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();

            return orders
                .SelectMany(o => o.OrderItems)
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
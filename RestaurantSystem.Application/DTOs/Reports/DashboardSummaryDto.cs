namespace RestaurantSystem.Application.DTOs.Reports
{
    // ملخص عام للأرقام الكبيرة (Summary Cards)
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; } // إجمالي الدخل
        public int TotalOrders { get; set; }      // إجمالي الطلبات
        public int TotalCustomers { get; set; }   // عدد الزبائن الفريدين
        public decimal AverageOrderValue => TotalOrders > 0 ? TotalRevenue / TotalOrders : 0;
    }

    // أكثر الوجبات مبيعاً
    public class TopMenuItemDto
    {
        public string Name { get; set; } = default!;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // المبيعات حسب الفئة (Chart البيانات)
    public class CategorySalesDto
    {
        public string CategoryName { get; set; } = default!;
        public decimal TotalSales { get; set; }
        public int ItemsSold { get; set; }
    }
}
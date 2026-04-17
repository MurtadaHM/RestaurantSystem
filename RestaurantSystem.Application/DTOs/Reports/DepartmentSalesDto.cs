namespace RestaurantSystem.Application.DTOs.Reports
{
    public class DepartmentSalesDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int ItemsSold { get; set; } // كم وجبة أو مشروب طلع من هذا القسم؟
    }
}
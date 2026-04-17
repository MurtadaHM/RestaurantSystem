using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Departments
{
    public class DepartmentResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public DepartmentStatus Status { get; set; }

        // يمكنك إضافة عدد الأصناف المربوطة بهذا القسم (اختياري للتقارير)
        public int MenuItemsCount { get; set; }
    }
}
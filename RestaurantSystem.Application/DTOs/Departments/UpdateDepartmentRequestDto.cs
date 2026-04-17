using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Departments
{
    public class UpdateDepartmentRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public DepartmentStatus Status { get; set; }
    }
}
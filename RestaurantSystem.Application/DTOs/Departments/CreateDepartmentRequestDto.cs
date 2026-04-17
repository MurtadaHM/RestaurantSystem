namespace RestaurantSystem.Application.DTOs.Departments
{
    public class CreateDepartmentRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
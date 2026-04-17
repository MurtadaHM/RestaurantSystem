using AutoMapper;
using RestaurantSystem.Application.DTOs.Departments;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentResponseDto>();
            CreateMap<CreateDepartmentRequestDto, Department>();
            CreateMap<UpdateDepartmentRequestDto, Department>();
        }
    }
}
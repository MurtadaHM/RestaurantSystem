using AutoMapper;
using RestaurantSystem.Application.DTOs.Categories;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // 1. CreateCategoryRequestDto → Category
            // نستخدم NewGuid و UtcNow فقط عند الإنشاء لأول مرة
            CreateMap<CreateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // 2. UpdateCategoryRequestDto → Category
            // نحدث فقط التاريخ ونترك المعرف وتاريخ الإنشاء كما هما
            CreateMap<UpdateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // 3. Category → CategoryResponseDto
            // 🚨 هنا السحر: نقوم بجلب اسم القسم من الـ Navigation Property
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : "غير مرتبط"))
                .ForMember(dest => dest.MenuItemCount,
                    opt => opt.MapFrom(src => src.MenuItems != null ? src.MenuItems.Count : 0));
        }
    }
}
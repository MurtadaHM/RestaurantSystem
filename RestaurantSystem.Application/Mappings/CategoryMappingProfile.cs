using AutoMapper;
using RestaurantSystem.Application.DTOs.Categories;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // ✅ CreateCategoryRequestDto → Category
            CreateMap<CreateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ✅ UpdateCategoryRequestDto → Category (للتحديث على entity موجود)
            CreateMap<UpdateCategoryRequestDto, Category>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())          // لا نعيد تعيين CreatedAt
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ✅ Category → CategoryResponseDto
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.MenuItemCount,
                    opt => opt.MapFrom(src =>
                        src.MenuItems != null ? src.MenuItems.Count : 0));
        }
    }
}

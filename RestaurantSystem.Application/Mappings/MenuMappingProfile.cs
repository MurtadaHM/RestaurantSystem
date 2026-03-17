using AutoMapper;
using RestaurantSystem.Application.DTOs.Menu;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class MenuMappingProfile : Profile
    {
        public MenuMappingProfile()
        {
            // ✅ CreateMenuItemRequestDto → MenuItem
            CreateMap<CreateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ✅ UpdateMenuItemRequestDto → MenuItem (للتحديث على entity موجود)
            CreateMap<UpdateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())          // لا نعيد تعيين CreatedAt
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ✅ MenuItem → MenuItemResponseDto
            CreateMap<MenuItem, MenuItemResponseDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src =>
                        src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.OrderCount,
                    opt => opt.MapFrom(src =>
                        src.OrderItems != null ? src.OrderItems.Count : 0));
        }
    }
}

using AutoMapper;
using RestaurantSystem.Application.DTOs.Menu;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class MenuMappingProfile : Profile
    {
        public MenuMappingProfile()
        {
            // 1. CreateMenuItemRequestDto → MenuItem
            // نقوم بتوليد Id جديد وتحديد تواريخ الإنشاء
            CreateMap<CreateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true));

            // 2. UpdateMenuItemRequestDto → MenuItem
            // نحدث حقل UpdatedAt ونحافظ على CreatedAt كما هو
            CreateMap<UpdateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // 3. MenuItem → MenuItemResponseDto
            // 🚀 هنا نقوم بربط الأسماء من العلاقات (Navigation Properties)
            CreateMap<MenuItem, MenuItemResponseDto>()
     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "بدون فئة"))
     .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : "غير محدد"))
     .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => 0)); // اجعلها 0 حالياً للتجربة
        }
    }
}
using AutoMapper;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Application.DTOs.Menu;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.DTOs.Tables;     // ✅ أضفنا هذا
using RestaurantSystem.Application.DTOs.Categories; // ✅ أضفنا هذا
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ──────────────────────────────────────────
            // 1. User & Auth Mappings
            // ──────────────────────────────────────────
            CreateMap<RegisterRequestDto, User>().ReverseMap();
            CreateMap<User, UserAuthDto>();

            // ──────────────────────────────────────────
            // 2. Table Mappings (حل مشكلة الخطأ 500)
            // ──────────────────────────────────────────
            CreateMap<Table, TableResponseDto>();
            CreateMap<CreateTableRequestDto, Table>();
            CreateMap<UpdateTableRequestDto, Table>(); // للاستخدام مستقبلاً في التحديث

            // ──────────────────────────────────────────
            // 3. Category Mappings
            // ──────────────────────────────────────────
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.MenuItemCount, opt => opt.MapFrom(src => src.MenuItems.Count));
            CreateMap<CreateCategoryRequestDto, Category>();

            // ──────────────────────────────────────────
            // 4. MenuItem Mappings
            // ──────────────────────────────────────────
            CreateMap<MenuItem, MenuItemResponseDto>();
            CreateMap<CreateMenuItemRequestDto, MenuItem>().ReverseMap();

            // ──────────────────────────────────────────
            // 5. Order Mappings
            // ──────────────────────────────────────────
            CreateMap<CreateOrderRequestDto, Order>();
            CreateMap<Order, OrderResponseDto>();
            CreateMap<OrderItem, OrderItemResponseDto>();
            CreateMap<CreateOrderItemDto, OrderItem>(); // مهم جداً لتحويل أصناف الطلب
        }
    }
}
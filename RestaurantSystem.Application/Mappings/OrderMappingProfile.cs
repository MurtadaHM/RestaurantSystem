using AutoMapper;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // ✅ Order → OrderResponseDto
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                        src.User != null
                            ? $"{src.User.FirstName} {src.User.LastName}"
                            : string.Empty))
                .ForMember(dest => dest.TableNumber,
                    opt => opt.MapFrom(src =>
                        src.Table != null ? src.Table.TableNumber : string.Empty))
                // ✅ تصحيح: اسم الخاصية في الـ Entity هو OrderType وليس Type
                .ForMember(dest => dest.OrderType,
                    opt => opt.MapFrom(src => src.OrderType.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Payment,
                    opt => opt.MapFrom(src => src.Payment));

            // ✅ OrderItem → OrderItemResponseDto
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.MenuItemName,
                    opt => opt.MapFrom(src =>
                        src.MenuItem != null ? src.MenuItem.Name : string.Empty))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price));

            // ✅ Payment → PaymentResponseDto
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                // ✅ تصحيح: اسم الخاصية في الـ Entity هو PaymentMethod وليس Method
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()));
        }
    }
}
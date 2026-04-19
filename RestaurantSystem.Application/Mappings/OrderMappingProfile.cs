using AutoMapper;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Order -> OrderResponseDto
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderNumber,
                    opt => opt.MapFrom(src => src.OrderNumber))

                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                        src.User != null
                            ? $"{src.User.FirstName} {src.User.LastName}".Trim()
                            : string.Empty))

                .ForMember(dest => dest.TableNumber,
                    opt => opt.MapFrom(src =>
                        src.Table != null
                            ? src.Table.TableNumber
                            : string.Empty))

                .ForMember(dest => dest.OrderType,
                    opt => opt.MapFrom(src => src.OrderType.ToString()))

                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))

                .ForMember(dest => dest.SpecialNotes,
                    opt => opt.MapFrom(src => src.SpecialNotes ?? string.Empty))

                .ForMember(dest => dest.DeliveryFee,
                    opt => opt.MapFrom(src => src.DeliveryFee ?? 0))

                // Sendy Integration
                .ForMember(dest => dest.ExternalDeliveryStatus,
                    opt => opt.MapFrom(src => src.ExternalDeliveryStatus.ToString()))

                .ForMember(dest => dest.ExternalOrderId,
                    opt => opt.MapFrom(src => src.ExternalOrderId))

                .ForMember(dest => dest.ExternalPublicId,
                    opt => opt.MapFrom(src => src.ExternalPublicId))

                .ForMember(dest => dest.IsSyncedToExternalProvider,
                    opt => opt.MapFrom(src => src.IsSyncedToExternalProvider))

                .ForMember(dest => dest.DeliveryAddress,
                    opt => opt.MapFrom(src => src.DeliveryAddress))

                .ForMember(dest => dest.CustomerPhoneNumber,
                    opt => opt.MapFrom(src => src.CustomerPhoneNumber))

                .ForMember(dest => dest.Latitude,
                    opt => opt.MapFrom(src => src.Latitude))

                .ForMember(dest => dest.Longitude,
                    opt => opt.MapFrom(src => src.Longitude))

                .ForMember(dest => dest.CourierName,
                    opt => opt.MapFrom(src => src.CourierName))

                .ForMember(dest => dest.CourierPhoneNumber,
                    opt => opt.MapFrom(src => src.CourierPhoneNumber))

                .ForMember(dest => dest.TrackingUrl,
                    opt => opt.MapFrom(src => src.TrackingUrl))

                .ForMember(dest => dest.LastExternalSyncDate,
                    opt => opt.MapFrom(src => src.LastExternalSyncDate))

                // Team 6 Integration
                .ForMember(dest => dest.PartnerOrderId,
                    opt => opt.MapFrom(src => src.PartnerOrderId))

                .ForMember(dest => dest.PartnerSource,
                    opt => opt.MapFrom(src => src.PartnerSource))

                .ForMember(dest => dest.PartnerRestaurantId,
                    opt => opt.MapFrom(src => src.PartnerRestaurantId))

                .ForMember(dest => dest.LastPartnerSyncDate,
                    opt => opt.MapFrom(src => src.LastPartnerSyncDate))

                .ForMember(dest => dest.PartnerUserId,
                    opt => opt.MapFrom(src => src.PartnerUserId))

                // Relations
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems))

                .ForMember(dest => dest.Payment,
                    opt => opt.MapFrom(src => src.Payment));

            // OrderItem -> OrderItemResponseDto
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.MenuItemName,
                    opt => opt.MapFrom(src =>
                        src.MenuItem != null
                            ? src.MenuItem.Name
                            : string.Empty))

                .ForMember(dest => dest.DepartmentId,
                    opt => opt.MapFrom(src => src.DepartmentId))

                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.Name
                            : (src.MenuItem != null && src.MenuItem.Department != null
                                ? src.MenuItem.Department.Name
                                : string.Empty)))

                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price))

                .ForMember(dest => dest.SpecialInstructions,
                    opt => opt.MapFrom(src => src.SpecialInstructions ?? string.Empty));

            // Payment -> PaymentResponseDto
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))

                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))

                .ForMember(dest => dest.TransactionReference,
                    opt => opt.MapFrom(src => src.TransactionReference ?? string.Empty))

                .ForMember(dest => dest.OrderNumber,
                    opt => opt.MapFrom(src =>
                        src.Order != null
                            ? src.Order.OrderNumber
                            : 0));
        }
    }
}
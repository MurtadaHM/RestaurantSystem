using AutoMapper;
using RestaurantSystem.Application.DTOs.Tables;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Mappings
{
    public class TableMappingProfile : Profile
    {
        public TableMappingProfile()
        {
            CreateMap<CreateTableRequestDto, Table>();
            CreateMap<UpdateTableRequestDto, Table>();

            CreateMap<Table, TableResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ActiveOrdersCount, opt => opt.MapFrom(src =>
                    src.Orders != null
                        ? src.Orders.Count(o =>
                            !o.IsDeleted &&
                            o.OrderType == OrderType.DineIn &&
                            o.Status != OrderStatus.Completed &&
                            o.Status != OrderStatus.Cancelled &&
                            o.Status != OrderStatus.Returned)
                        : 0));
        }
    }
}
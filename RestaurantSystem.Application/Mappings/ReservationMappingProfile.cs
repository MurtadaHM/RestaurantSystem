using AutoMapper;
using RestaurantSystem.Application.DTOs.Reservation;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class ReservationMappingProfile : Profile
    {
        public ReservationMappingProfile()
        {
            // من الطلب إلى الكيان (Entity)
            CreateMap<CreateReservationRequestDto, Reservation>();
            CreateMap<UpdateReservationRequestDto, Reservation>();

            // من الكيان إلى استجابة الواجهة (Response)
            CreateMap<Reservation, ReservationResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table != null ? src.Table.TableNumber : "غير محدد"));
        }
    }
}
using AutoMapper;
using RestaurantSystem.Application.DTOs.Payments;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            // ✅ CreatePaymentRequestDto → Payment
            CreateMap<CreatePaymentRequestDto, Payment>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore()) // نحدده يدوياً في الـ Service
                .ForMember(dest => dest.PaymentDate,
                    opt => opt.Ignore()); // null حتى يتم الدفع

            // ✅ Payment → PaymentResponseDto
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OrderTotalAmount,
                    opt => opt.MapFrom(src =>
                        src.Order != null ? src.Order.TotalAmount : 0));
        }
    }
}

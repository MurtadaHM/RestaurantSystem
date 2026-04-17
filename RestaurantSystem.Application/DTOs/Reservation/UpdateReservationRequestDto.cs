using System;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.Reservation
{
    public class UpdateReservationRequestDto
    {
        public Guid Id { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ReservationDate { get; set; }
        public int GuestCount { get; set; }
        public Guid TableId { get; set; }
        public string? SpecialRequests { get; set; }
        public string? PreparationNotes { get; set; }
    }
}
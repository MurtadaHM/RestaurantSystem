using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.PublicOrders
{
    public class CreatePublicTableOrderRequestDto
    {
        public string? CustomerName { get; set; }

        [Required]
        [Phone]
        public string CustomerPhone { get; set; } = string.Empty;

        public string? SpecialNotes { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreatePublicOrderItemDto> Items { get; set; } = new();
    }

   
}
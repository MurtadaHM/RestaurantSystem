
using System.ComponentModel.DataAnnotations;


namespace RestaurantSystem.Application.DTOs.PublicOrders
{
    public class CreatePublicOrderItemDto
    {
        [Required]
        public Guid MenuItemId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public string? SpecialInstructions { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Application.DTOs.Tables
{
    public class UpdateTableRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string TableNumber { get; set; } = default!;

        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = default!;

        [Required]
        [Range(1, 100)]
        public int Capacity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = default!;

        [MaxLength(50)]
        public string? Zone { get; set; }

        public int? FloorNumber { get; set; }

        public bool IsActive { get; set; }

        public bool IsOrderingEnabled { get; set; }
    }
}
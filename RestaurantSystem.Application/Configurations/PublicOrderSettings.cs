using System;

namespace RestaurantSystem.Application.Configurations
{
    public class PublicOrderSettings
    {
        public Guid FallbackUserId { get; set; } =
            Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    }
}
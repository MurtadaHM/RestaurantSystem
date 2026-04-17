using AutoMapper;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    // ✅ تغيير الوصول إلى public والوراثة من Profile
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            // 1️⃣ من Ingredient (Entity) إلى IngredientResponseDto (للعرض)
            CreateMap<Ingredient, IngredientResponseDto>()
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));

            // 2️⃣ من CreateIngredientRequestDto إلى Ingredient (للحفظ أول مرة)
            CreateMap<CreateIngredientRequestDto, Ingredient>()
                .ForMember(dest => dest.CurrentStock, opt => opt.MapFrom(src => src.InitialStock));

            // 3️⃣ خرائط الوصفة (MenuItemIngredient)
            CreateMap<MenuItemIngredient, MenuItemIngredientDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient!.Name));

            CreateMap<MenuItemIngredientDto, MenuItemIngredient>();

            // 4️⃣ خرائط حركات المخزن (StockMovements)
            CreateMap<StockMovement, StockMovementResponseDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient!.Name))
                .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
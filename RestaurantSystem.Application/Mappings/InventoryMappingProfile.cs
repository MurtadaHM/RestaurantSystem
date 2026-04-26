using AutoMapper;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Mappings
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            // 1️⃣ Ingredient -> IngredientResponseDto
            CreateMap<Ingredient, IngredientResponseDto>()
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src => src.Unit.ToString()));

            // 2️⃣ CreateIngredientRequestDto -> Ingredient
            CreateMap<CreateIngredientRequestDto, Ingredient>()
                .ForMember(dest => dest.CurrentStock,
                    opt => opt.MapFrom(src => src.InitialStock));

            // 3️⃣ MenuItemIngredient -> MenuItemIngredientDto
            CreateMap<MenuItemIngredient, MenuItemIngredientDto>()
                .ForMember(dest => dest.IngredientName,
                    opt => opt.MapFrom(src =>
                        src.Ingredient != null ? src.Ingredient.Name : string.Empty))

                // الوحدة للعرض فقط من Ingredient، مو مخزونة بالوصفة
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src =>
                        src.Ingredient != null ? src.Ingredient.Unit.ToString() : string.Empty))

                .ForMember(dest => dest.Notes,
                    opt => opt.MapFrom(src => src.Notes))

                .ForMember(dest => dest.IsOptional,
                    opt => opt.MapFrom(src => src.IsOptional))

                .ForMember(dest => dest.WastePercentage,
                    opt => opt.MapFrom(src => src.WastePercentage));

            // 4️⃣ MenuItemIngredientDto -> MenuItemIngredient
            CreateMap<MenuItemIngredientDto, MenuItemIngredient>()
                .ForMember(dest => dest.Ingredient,
                    opt => opt.Ignore())

                .ForMember(dest => dest.MenuItem,
                    opt => opt.Ignore())

                .ForMember(dest => dest.Notes,
                    opt => opt.MapFrom(src => src.Notes))

                .ForMember(dest => dest.IsOptional,
                    opt => opt.MapFrom(src => src.IsOptional))

                .ForMember(dest => dest.WastePercentage,
                    opt => opt.MapFrom(src => src.WastePercentage));

            // 5️⃣ StockMovement -> StockMovementResponseDto
            CreateMap<StockMovement, StockMovementResponseDto>()
                .ForMember(dest => dest.IngredientName,
                    opt => opt.MapFrom(src =>
                        src.Ingredient != null ? src.Ingredient.Name : string.Empty))

                .ForMember(dest => dest.MovementType,
                    opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
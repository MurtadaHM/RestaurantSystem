using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Exceptions;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IIngredientRepository ingredientRepo,
            IOrderRepository orderRepo,
            IMapper mapper,
            ILogger<InventoryService> logger)
        {
            _ingredientRepo = ingredientRepo;
            _orderRepo = orderRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<IngredientResponseDto>> GetAllIngredientsAsync()
        {
            var ingredients = await _ingredientRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<IngredientResponseDto>>(ingredients);
        }

        public async Task<IngredientResponseDto> GetIngredientByIdAsync(Guid id)
        {
            var ingredient = await _ingredientRepo.GetByIdAsync(id);
            return _mapper.Map<IngredientResponseDto>(ingredient);
        }

        public async Task<IngredientResponseDto> CreateIngredientAsync(CreateIngredientRequestDto request)
        {
            var ingredient = _mapper.Map<Ingredient>(request);
            ingredient.CurrentStock = request.InitialStock;

            if (ingredient.StockMovements == null)
                ingredient.StockMovements = new List<StockMovement>();

            ingredient.StockMovements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                Quantity = request.InitialStock,
                Type = MovementType.Purchase,
                Reason = "رصيد افتتاح عند تعريف المادة",
                MovementDate = DateTime.UtcNow
            });

            await _ingredientRepo.AddAsync(ingredient);
            await _ingredientRepo.SaveChangesAsync();

            _logger.LogInformation(
                "✨ تم تعريف {Name} برصيد افتتاحي {Qty}",
                ingredient.Name,
                ingredient.CurrentStock);

            return _mapper.Map<IngredientResponseDto>(ingredient);
        }

        public async Task<bool> UpdateRecipeAsync(Guid menuItemId, List<MenuItemIngredientDto> ingredients)
        {
            try
            {
                _logger.LogInformation("🔄 تحديث وصفة الطبق {Id}", menuItemId);

                // حذف فعلي للوصفة القديمة لتجنب duplicate key
                await _ingredientRepo.HardDeleteRecipeByMenuItemIdAsync(menuItemId);
                await _ingredientRepo.SaveChangesAsync();

                foreach (var item in ingredients)
                {
                    var mapping = new MenuItemIngredient
                    {
                        Id = Guid.NewGuid(),
                        MenuItemId = menuItemId,
                        IngredientId = item.IngredientId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        Notes = item.Notes,
                        IsOptional = item.IsOptional,
                        WastePercentage = item.WastePercentage
                    };

                    await _ingredientRepo.AddRecipeItemAsync(mapping);
                }

                await _ingredientRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ فشل تحديث وصفة الطبق {Id}", menuItemId);
                throw;
            }
        }

        public async Task<IEnumerable<MenuItemIngredientDto>> GetRecipeByMenuItemIdAsync(Guid menuItemId)
        {
            var recipe = await _ingredientRepo.GetRecipeByMenuItemIdInternalAsync(menuItemId);
            return _mapper.Map<IEnumerable<MenuItemIngredientDto>>(recipe);
        }

        public async Task<IEnumerable<StockMovementResponseDto>> GetStockHistoryAsync(Guid ingredientId)
        {
            var history = await _ingredientRepo.GetStockHistoryAsync(ingredientId);
            return _mapper.Map<IEnumerable<StockMovementResponseDto>>(history);
        }

        public async Task<bool> ProcessOrderStockDeductionAsync(Guid orderId)
        {
            var order = await _orderRepo.GetOrderWithDetailsForInventoryAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("❌ لم يتم العثور على الطلب {OrderId}", orderId);
                return false;
            }

            if (order.IsStockDeducted)
            {
                _logger.LogInformation(
                    "ℹ️ تم تجاوز الخصم للطلب #{OrderNo} لأن الخصم منفذ مسبقاً",
                    order.OrderNumber);
                return true;
            }

            var requiredIngredients = new Dictionary<Guid, (Ingredient Ingredient, decimal RequiredQuantity)>();

            foreach (var item in order.OrderItems)
            {
                if (item.MenuItem?.MenuItemIngredients == null)
                    continue;

                foreach (var recipeItem in item.MenuItem.MenuItemIngredients)
                {
                    var ingredient = recipeItem.Ingredient;
                    if (ingredient == null)
                        continue;

                    decimal baseRequired = item.Quantity * recipeItem.Quantity;
                    decimal wasteAmount = baseRequired * (recipeItem.WastePercentage / 100m);
                    decimal totalRequired = baseRequired + wasteAmount;

                    if (requiredIngredients.TryGetValue(ingredient.Id, out var existing))
                    {
                        requiredIngredients[ingredient.Id] =
                            (existing.Ingredient, existing.RequiredQuantity + totalRequired);
                    }
                    else
                    {
                        requiredIngredients[ingredient.Id] = (ingredient, totalRequired);
                    }
                }
            }

            var insufficientIngredients = new List<string>();

            foreach (var kvp in requiredIngredients)
            {
                var ingredient = kvp.Value.Ingredient;
                var required = kvp.Value.RequiredQuantity;

                if (ingredient.CurrentStock < required)
                {
                    insufficientIngredients.Add(
                        $"{ingredient.Name}: المطلوب {required}, المتوفر {ingredient.CurrentStock}");
                }
            }

            if (insufficientIngredients.Any())
            {
                var message =
                    "المخزون غير كافٍ للمواد التالية: " +
                    string.Join(" | ", insufficientIngredients);

                _logger.LogWarning(
                    "⚠️ فشل خصم المخزون للطلب #{OrderNo}: {Message}",
                    order.OrderNumber,
                    message);

                throw new ValidationException(message);
            }

            foreach (var kvp in requiredIngredients)
            {
                var ingredient = kvp.Value.Ingredient;
                var totalToDeduct = kvp.Value.RequiredQuantity;

                ingredient.CurrentStock -= totalToDeduct;

                if (ingredient.StockMovements == null)
                    ingredient.StockMovements = new List<StockMovement>();

                ingredient.StockMovements.Add(new StockMovement
                {
                    Id = Guid.NewGuid(),
                    IngredientId = ingredient.Id,
                    Quantity = totalToDeduct,
                    Type = MovementType.Sale,
                    Reason = $"خصم مبيعات - طلب #{order.OrderNumber}",
                    MovementDate = DateTime.UtcNow
                });

                _logger.LogInformation(
                    "✅ تم خصم {Qty} من المادة {IngredientName} للطلب #{OrderNo}",
                    totalToDeduct,
                    ingredient.Name,
                    order.OrderNumber);
            }

            order.IsStockDeducted = true;

            await _ingredientRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddStockAsync(AddStockRequestDto request)
        {
            var ingredient = await _ingredientRepo.GetByIdAsync(request.IngredientId);
            if (ingredient == null)
                return false;

            ingredient.CurrentStock += request.Quantity;

            if (ingredient.StockMovements == null)
                ingredient.StockMovements = new List<StockMovement>();

            ingredient.StockMovements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                IngredientId = ingredient.Id,
                Quantity = request.Quantity,
                Type = MovementType.Purchase,
                Reason = request.Notes ?? "تجهيز مخزون جديد",
                MovementDate = DateTime.UtcNow
            });

            await _ingredientRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IngredientResponseDto>> GetLowStockIngredientsAsync()
        {
            var ingredients = await _ingredientRepo.GetAllAsync();

            return _mapper.Map<IEnumerable<IngredientResponseDto>>(
                ingredients.Where(i => i.CurrentStock <= i.MinThreshold && !i.IsDeleted));
        }

        public async Task<bool> UpdateIngredientAsync(Guid id, CreateIngredientRequestDto request)
        {
            var ingredient = await _ingredientRepo.GetByIdAsync(id);
            if (ingredient == null)
                return false;

            ingredient.Name = request.Name;
            ingredient.Unit = request.Unit;
            ingredient.MinThreshold = request.MinThreshold;
            ingredient.UnitPrice = request.UnitPrice;
            ingredient.CurrentStock = request.InitialStock;

            _ingredientRepo.Update(ingredient);
            await _ingredientRepo.SaveChangesAsync();
            return true;
        }
    }
}
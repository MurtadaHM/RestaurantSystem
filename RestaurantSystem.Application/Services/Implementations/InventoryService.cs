using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

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

        // 1️⃣ إضافة مادة جديدة مع معالجة الكمية الابتدائية 🔥
        public async Task<IngredientResponseDto> CreateIngredientAsync(CreateIngredientRequestDto request)
        {
            var ingredient = _mapper.Map<Ingredient>(request);

            // الربط اليدوي للكمية لأن الأسماء تختلف (InitialStock -> CurrentStock)
            ingredient.CurrentStock = request.InitialStock;

            // إضافة حركة مخزنية كرصيد أول المدة
            if (ingredient.StockMovements == null)
                ingredient.StockMovements = new List<StockMovement>();

            ingredient.StockMovements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                Quantity = request.InitialStock,
                Type = MovementType.Purchase,
                Reason = "رصيد افتتاح عند تعريف المادة",
                MovementDate = DateTime.Now
            });

            await _ingredientRepo.AddAsync(ingredient);
            await _ingredientRepo.SaveChangesAsync();

            _logger.LogInformation("✨ تم تعريف {Name} برصيد افتتاحي {Qty}", ingredient.Name, ingredient.CurrentStock);

            return _mapper.Map<IngredientResponseDto>(ingredient);
        }

        // 2️⃣ ميثود تحديث الوصفة (الربط بين الطبق والمكونات) 🍕
        public async Task<bool> UpdateRecipeAsync(Guid menuItemId, List<MenuItemIngredientDto> ingredients)
        {
            try
            {
                _logger.LogInformation("🔄 تحديث وصفة الطبق {Id}", menuItemId);

                // أ. حذف الوصفة القديمة لتجنب التكرار
                var existingRecipe = await _ingredientRepo.GetRecipeByMenuItemIdInternalAsync(menuItemId);
                if (existingRecipe.Any())
                {
                    await _ingredientRepo.RemoveRangeAsync(existingRecipe);
                }

                // ب. إضافة المكونات الجديدة للوصفة
                foreach (var item in ingredients)
                {
                    var mapping = new MenuItemIngredient
                    {
                        Id = Guid.NewGuid(),
                        MenuItemId = menuItemId,
                        IngredientId = item.IngredientId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _ingredientRepo.AddRecipeItemAsync(mapping);
                }

                await _ingredientRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ فشل تحديث وصفة الطبق {Id}", menuItemId);
                return false;
            }
        }

        // 3️⃣ جلب وصفة طبق معين
        public async Task<IEnumerable<MenuItemIngredientDto>> GetRecipeByMenuItemIdAsync(Guid menuItemId)
        {
            var recipe = await _ingredientRepo.GetRecipeByMenuItemIdInternalAsync(menuItemId);
            return _mapper.Map<IEnumerable<MenuItemIngredientDto>>(recipe);
        }

        // 4️⃣ جلب سجل حركات المادة (لزر السجل 📜)
        public async Task<IEnumerable<StockMovementResponseDto>> GetStockHistoryAsync(Guid ingredientId)
        {
            var history = await _ingredientRepo.GetStockHistoryAsync(ingredientId);
            return _mapper.Map<IEnumerable<StockMovementResponseDto>>(history);
        }

        // --- محرك الخصم التلقائي عند الطلب (كما هو) ---
        public async Task<bool> ProcessOrderStockDeductionAsync(Guid orderId)
        {
            var order = await _orderRepo.GetOrderWithDetailsForInventoryAsync(orderId);
            if (order == null) return false;

            foreach (var item in order.OrderItems)
            {
                if (item.MenuItem?.MenuItemIngredients == null) continue;

                foreach (var recipeItem in item.MenuItem.MenuItemIngredients)
                {
                    var ingredient = recipeItem.Ingredient;
                    if (ingredient == null) continue;

                    decimal totalToDeduct = item.Quantity * recipeItem.Quantity;
                    ingredient.CurrentStock -= totalToDeduct;

                    ingredient.StockMovements.Add(new StockMovement
                    {
                        IngredientId = ingredient.Id,
                        Quantity = totalToDeduct,
                        Type = MovementType.Sale,
                        Reason = $"خصم مبيعات - طلب #{order.OrderNumber}",
                        MovementDate = DateTime.Now
                    });
                }
            }

            await _ingredientRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddStockAsync(AddStockRequestDto request)
        {
            var ingredient = await _ingredientRepo.GetByIdAsync(request.IngredientId);
            if (ingredient == null) return false;

            ingredient.CurrentStock += request.Quantity;
            ingredient.StockMovements.Add(new StockMovement
            {
                IngredientId = ingredient.Id,
                Quantity = request.Quantity,
                Type = MovementType.Purchase,
                Reason = request.Notes ?? "تجهيز مخزون جديد",
                MovementDate = DateTime.Now
            });

            await _ingredientRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IngredientResponseDto>> GetLowStockIngredientsAsync()
        {
            var ingredients = await _ingredientRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<IngredientResponseDto>>(
                ingredients.Where(i => i.CurrentStock <= i.MinThreshold && !i.IsDeleted)
            );
        }

        public async Task<bool> UpdateIngredientAsync(Guid id, CreateIngredientRequestDto request)
        {
            var ingredient = await _ingredientRepo.GetByIdAsync(id);
            if (ingredient == null) return false;

            ingredient.Name = request.Name;
            ingredient.Unit = request.Unit;
            ingredient.MinThreshold = request.MinThreshold;
            ingredient.UnitPrice = request.UnitPrice;
            ingredient.CurrentStock = request.InitialStock; // 🔥 إضافة هذا السطر للسماح بتعديل الكمية مباشرة
            _ingredientRepo.Update(ingredient);
            await _ingredientRepo.SaveChangesAsync();
            return true;
        }
    }
}
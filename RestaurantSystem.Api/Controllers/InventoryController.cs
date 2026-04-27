using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _inventoryService.GetAllIngredientsAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _inventoryService.GetIngredientByIdAsync(id);

            if (result == null)
                return NotFound(new { success = false, message = "المادة المطلوبة غير موجودة" });

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIngredientRequestDto request)
        {
            var result = await _inventoryService.CreateIngredientAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                new { success = true, data = result });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateIngredientRequestDto request)
        {
            var success = await _inventoryService.UpdateIngredientAsync(id, request);

            if (!success)
                return BadRequest(new { success = false, message = "فشلت عملية التعديل" });

            return Ok(new { success = true, message = "تم تحديث بيانات المادة بنجاح" });
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] AddStockRequestDto request)
        {
            var success = await _inventoryService.AddStockAsync(request);

            if (!success)
                return BadRequest(new { success = false, message = "فشلت عملية إضافة الكمية، تأكد من معرف المادة" });

            return Ok(new { success = true, message = "تم تحديث المخزن بنجاح" });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _inventoryService.GetLowStockIngredientsAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpGet("recipe/{menuItemId:guid}")]
        public async Task<IActionResult> GetRecipe(Guid menuItemId)
        {
            var result = await _inventoryService.GetRecipeByMenuItemIdAsync(menuItemId);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("recipe/{menuItemId:guid}")]
        public async Task<IActionResult> UpdateRecipe(
            Guid menuItemId,
            [FromBody] List<MenuItemIngredientDto> ingredients)
        {
            var success = await _inventoryService.UpdateRecipeAsync(menuItemId, ingredients);

            if (!success)
                return BadRequest(new { success = false, message = "فشلت عملية تحديث الوصفة" });

            return Ok(new { success = true, message = "تم تحديث وصفة الطبق بنجاح" });
        }

        [HttpGet("{id:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var result = await _inventoryService.GetStockHistoryAsync(id);
            return Ok(new { success = true, data = result });
        }
    }
}
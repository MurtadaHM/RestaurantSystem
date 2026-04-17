using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.DTOs.Inventory;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    // [Authorize] // يمكنك تفعيلها لاحقاً لتحديد الصلاحيات
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // 1. جلب كل المواد الأولية - معدل ليتوافق مع الـ JS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _inventoryService.GetAllIngredientsAsync();
            return Ok(new { success = true, data = result });
        }

        // 2. جلب مادة معينة بالـ ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _inventoryService.GetIngredientByIdAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "المادة المطلوبة غير موجودة" });

            return Ok(new { success = true, data = result });
        }

        // 3. إضافة مادة جديدة لأول مرة (تأكد أن الـ Request يحتوي على الكمية الابتدائية)
        [HttpPost]
        public async Task<IActionResult> Create(CreateIngredientRequestDto request)
        {
            var result = await _inventoryService.CreateIngredientAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
        }

        // 4. إضافة مشتريات (زيادة الكمية الموجودة)
        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock(AddStockRequestDto request)
        {
            var success = await _inventoryService.AddStockAsync(request);
            if (!success)
                return BadRequest(new { success = false, message = "فشلت عملية إضافة الكمية، تأكد من معرف المادة" });

            return Ok(new { success = true, message = "تم تحديث المخزن بنجاح" });
        }

        // 5. جلب المواد التي وصلت لخط الخطر (Low Stock)
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _inventoryService.GetLowStockIngredientsAsync();
            return Ok(new { success = true, data = result });
        }

        // 6. تحديث وصفة طبق معين
        [HttpPut("recipe/{menuItemId}")]
        public async Task<IActionResult> UpdateRecipe(Guid menuItemId, [FromBody] List<MenuItemIngredientDto> ingredients)
        {
            var success = await _inventoryService.UpdateRecipeAsync(menuItemId, ingredients);
            if (!success)
                return BadRequest(new { success = false, message = "فشلت عملية تحديث الوصفة" });

            return Ok(new { success = true, message = "تم تحديث وصفة الطبق بنجاح" });
        }

        // 7. جلب وصفة طبق معين
        [HttpGet("recipe/{menuItemId}")]
        public async Task<IActionResult> GetRecipe(Guid menuItemId)
        {
            var result = await _inventoryService.GetRecipeByMenuItemIdAsync(menuItemId);
            return Ok(new { success = true, data = result });
        }

        // 8. جلب تاريخ حركات مادة معينة (Auditing)
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var result = await _inventoryService.GetStockHistoryAsync(id);
            return Ok(new { success = true, data = result });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateIngredientRequestDto request)
        {
            var success = await _inventoryService.UpdateIngredientAsync(id, request);
            if (!success) return BadRequest(new { success = false, message = "فشلت عملية التعديل" });
            return Ok(new { success = true, message = "تم تحديث بيانات المادة بنجاح" });
        }


    }
}
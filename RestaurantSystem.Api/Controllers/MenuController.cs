using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Menu;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة المنيو والمنتجات
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(
            IMenuService menuService,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        // ──────────────────────────────────────────
        // GET /api/v1/menu
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب جميع المنتجات
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuItemResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MenuItemResponseDto>>>> GetAllMenuItems()
        {
            _logger.LogInformation("Fetching all menu items");
            var items = await _menuService.GetAllMenuItemsAsync();
            return Ok(ApiResponse<IEnumerable<MenuItemResponseDto>>.Ok(items));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/menu/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب منتج محدد بالـ ID
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MenuItemResponseDto>>> GetMenuItem(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetMenuItem called with empty ID");
                return BadRequest(ApiResponse<MenuItemResponseDto>.Fail("معرف المنتج غير صحيح"));
            }

            _logger.LogInformation("Fetching menu item: {MenuItemId}", id);
            var item = await _menuService.GetMenuItemByIdAsync(id);
            return Ok(ApiResponse<MenuItemResponseDto>.Ok(item));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/menu/category/{categoryId}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب منتجات فئة معينة
        /// </summary>
        [AllowAnonymous]
        [HttpGet("category/{categoryId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuItemResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MenuItemResponseDto>>>> GetMenuItemsByCategory(
            Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                _logger.LogWarning("GetMenuItemsByCategory called with empty categoryId");
                return BadRequest(ApiResponse<IEnumerable<MenuItemResponseDto>>.Fail("معرف الفئة غير صحيح"));
            }

            _logger.LogInformation("Fetching menu items for category: {CategoryId}", categoryId);
            var items = await _menuService.GetMenuItemsByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<MenuItemResponseDto>>.Ok(items));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/menu/search?term=...
        // ──────────────────────────────────────────
        /// <summary>
        /// البحث في المنيو بالاسم
        /// </summary>
        [AllowAnonymous]
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuItemResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MenuItemResponseDto>>>> SearchMenuItems(
            [FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                _logger.LogWarning("SearchMenuItems called with empty search term");
                return BadRequest(ApiResponse<IEnumerable<MenuItemResponseDto>>.Fail("كلمة البحث مطلوبة"));
            }

            _logger.LogInformation("Searching menu items: {SearchTerm}", term);
            var items = await _menuService.SearchMenuItemsAsync(term);
            return Ok(ApiResponse<IEnumerable<MenuItemResponseDto>>.Ok(items));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/menu
        // ──────────────────────────────────────────
        /// <summary>
        /// إضافة منتج جديد للمنيو
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<MenuItemResponseDto>>> CreateMenuItem(
            [FromBody] CreateMenuItemRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateMenuItem called with null request");
                return BadRequest(ApiResponse<MenuItemResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation("Creating menu item: {MenuItemName}", request.Name);
            var result = await _menuService.CreateMenuItemAsync(request);

            _logger.LogInformation("Menu item created: {MenuItemId}", result.Id);
            return Ok(ApiResponse<MenuItemResponseDto>.Ok(result, "تم إضافة المنتج بنجاح"));
        }

        // ──────────────────────────────────────────
        // PUT /api/v1/menu/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// تعديل منتج موجود
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<MenuItemResponseDto>>> UpdateMenuItem(
            Guid id,
            [FromBody] UpdateMenuItemRequestDto request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateMenuItem called with empty ID");
                return BadRequest(ApiResponse<MenuItemResponseDto>.Fail("معرف المنتج غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdateMenuItem called with null request");
                return BadRequest(ApiResponse<MenuItemResponseDto>.Fail("البيانات مطلوبة"));
            }

            // ✅ تأكد تطابق ID من URL مع Body
            if (request.Id != Guid.Empty && request.Id != id)
            {
                _logger.LogWarning(
                    "UpdateMenuItem ID mismatch: URL={UrlId}, Body={BodyId}", id, request.Id);
                return BadRequest(ApiResponse<MenuItemResponseDto>.Fail("معرف المنتج غير متطابق"));
            }

            // ✅ نضع الـ ID من الـ URL دائماً
            request.Id = id;

            _logger.LogInformation("Updating menu item: {MenuItemId}", id);
            var result = await _menuService.UpdateMenuItemAsync(id, request);

            _logger.LogInformation("Menu item updated: {MenuItemId}", id);
            return Ok(ApiResponse<MenuItemResponseDto>.Ok(result, "تم تعديل المنتج بنجاح"));
        }

        // ──────────────────────────────────────────
        // DELETE /api/v1/menu/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// حذف منتج من المنيو
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMenuItem(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteMenuItem called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف المنتج غير صحيح"));
            }

            _logger.LogInformation("Deleting menu item: {MenuItemId}", id);
            var result = await _menuService.DeleteMenuItemAsync(id);

            if (!result)
            {
                _logger.LogWarning("Delete failed, menu item not found: {MenuItemId}", id);
                return BadRequest(ApiResponse<object>.Fail("المنتج غير موجود"));
            }

            _logger.LogInformation("Menu item deleted: {MenuItemId}", id);
            return Ok(ApiResponse<object>.Ok("تم حذف المنتج بنجاح"));
        }
    }
}

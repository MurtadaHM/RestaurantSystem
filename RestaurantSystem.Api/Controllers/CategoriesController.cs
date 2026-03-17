using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Categories;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة فئات المنيو
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]   // ✅ api/v1/categories
    [Produces("application/json")]
    [Tags("Categories")]
    public class CategoriesController : ControllerBase  // ✅ الاسم الصحيح
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // ──────────────────────────────────────────
        // GET /api/v1/categories
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب جميع الفئات مع عدد منتجاتها
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponseDto>>>> GetAllCategories()
        {
            _logger.LogInformation("Fetching all categories");
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(categories));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/categories/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب فئة محددة بالـ ID
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetCategory called with empty ID");
                return BadRequest(ApiResponse<CategoryResponseDto>.Fail("معرف الفئة غير صحيح"));
            }

            _logger.LogInformation("Fetching category: {CategoryId}", id);
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(ApiResponse<CategoryResponseDto>.Ok(category));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/categories
        // ──────────────────────────────────────────
        /// <summary>
        /// إضافة فئة جديدة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> CreateCategory(
            [FromBody] CreateCategoryRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateCategory called with null request");
                return BadRequest(ApiResponse<CategoryResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation("Creating category: {CategoryName}", request.Name);
            var result = await _categoryService.CreateCategoryAsync(request);

            _logger.LogInformation("Category created: {CategoryId}", result.Id);
            return Ok(ApiResponse<CategoryResponseDto>.Ok(result, "تم إضافة الفئة بنجاح"));
        }

        // ──────────────────────────────────────────
        // PUT /api/v1/categories/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// تعديل فئة موجودة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> UpdateCategory(
            Guid id,
            [FromBody] UpdateCategoryRequestDto request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateCategory called with empty ID");
                return BadRequest(ApiResponse<CategoryResponseDto>.Fail("معرف الفئة غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdateCategory called with null request");
                return BadRequest(ApiResponse<CategoryResponseDto>.Fail("البيانات مطلوبة"));
            }

            // ✅ تأكد تطابق ID من URL مع Body
            if (request.Id != Guid.Empty && request.Id != id)
            {
                _logger.LogWarning(
                    "UpdateCategory ID mismatch: URL={UrlId}, Body={BodyId}", id, request.Id);
                return BadRequest(ApiResponse<CategoryResponseDto>.Fail("معرف الفئة غير متطابق"));
            }

            // ✅ نضع الـ ID من الـ URL دائماً
            request.Id = id;

            _logger.LogInformation("Updating category: {CategoryId}", id);
            var result = await _categoryService.UpdateCategoryAsync(id, request);

            _logger.LogInformation("Category updated: {CategoryId}", id);
            return Ok(ApiResponse<CategoryResponseDto>.Ok(result, "تم تعديل الفئة بنجاح"));
        }

        // ──────────────────────────────────────────
        // DELETE /api/v1/categories/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// حذف فئة — فقط لو ما فيها منتجات
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteCategory called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف الفئة غير صحيح"));
            }

            _logger.LogInformation("Deleting category: {CategoryId}", id);
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
            {
                _logger.LogWarning("Delete failed, category not found: {CategoryId}", id);
                return BadRequest(ApiResponse<object>.Fail("الفئة غير موجودة"));
            }

            _logger.LogInformation("Category deleted: {CategoryId}", id);
            return Ok(ApiResponse<object>.Ok("تم حذف الفئة بنجاح"));
        }
    }
}

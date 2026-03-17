using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Tables;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة الطاولات
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Tables")]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly ILogger<TablesController> _logger;

        public TablesController(
            ITableService tableService,
            ILogger<TablesController> logger)
        {
            _tableService = tableService;
            _logger = logger;
        }

        /// <summary>
        /// الحصول على كل الطاولات
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TableResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<IEnumerable<TableResponseDto>>>> GetAllTables()
        {
            _logger.LogInformation("Fetching all tables");
            var tables = await _tableService.GetAllTablesAsync();
            return Ok(ApiResponse<IEnumerable<TableResponseDto>>.Ok(tables));
        }

        /// <summary>
        /// الحصول على الطاولات المتاحة فقط
        /// </summary>
        [Authorize]
        [HttpGet("available")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TableResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<IEnumerable<TableResponseDto>>>> GetAvailableTables()
        {
            _logger.LogInformation("Fetching available tables");
            var tables = await _tableService.GetAvailableTablesAsync();
            return Ok(ApiResponse<IEnumerable<TableResponseDto>>.Ok(tables));
        }

        /// <summary>
        /// الحصول على طاولة محددة
        /// </summary>
        [Authorize]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<TableResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<TableResponseDto>>> GetTable(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetTable called with empty ID");
                return BadRequest(ApiResponse<TableResponseDto>.Fail("معرف الطاولة غير صحيح"));
            }

            _logger.LogInformation("Fetching table: {TableId}", id);
            var table = await _tableService.GetTableByIdAsync(id);
            return Ok(ApiResponse<TableResponseDto>.Ok(table));
        }

        /// <summary>
        /// إضافة طاولة جديدة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TableResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<TableResponseDto>>> CreateTable(
            [FromBody] CreateTableRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateTable called with null request");
                return BadRequest(ApiResponse<TableResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation("Creating new table: {TableNumber}", request.TableNumber);
            var result = await _tableService.CreateTableAsync(request);
            _logger.LogInformation("Table created: {TableId}", result.Id);

            return Ok(ApiResponse<TableResponseDto>.Ok(result, "تم إضافة الطاولة بنجاح"));
        }

        /// <summary>
        /// تعديل بيانات الطاولة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<TableResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<TableResponseDto>>> UpdateTable(
            Guid id,
            [FromBody] UpdateTableRequestDto request)  // ✅ الاسم الصحيح
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateTable called with empty ID");
                return BadRequest(ApiResponse<TableResponseDto>.Fail("معرف الطاولة غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdateTable called with null request");
                return BadRequest(ApiResponse<TableResponseDto>.Fail("البيانات مطلوبة"));
            }

            // ✅ تأكد إن الـ ID في الـ URL يطابق الـ ID في الـ Body
            if (request.Id != Guid.Empty && request.Id != id)
            {
                return BadRequest(ApiResponse<TableResponseDto>.Fail("معرف الطاولة غير متطابق"));
            }

            // ✅ نضع الـ ID من الـ URL في الـ request دائماً
            request.Id = id;

            _logger.LogInformation("Updating table: {TableId}", id);
            var result = await _tableService.UpdateTableAsync(id, request);
            _logger.LogInformation("Table updated: {TableId}", id);

            return Ok(ApiResponse<TableResponseDto>.Ok(result, "تم تعديل الطاولة بنجاح"));
        }

        /// <summary>
        /// حذف طاولة
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTable(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteTable called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف الطاولة غير صحيح"));
            }

            _logger.LogInformation("Deleting table: {TableId}", id);
            await _tableService.DeleteTableAsync(id);
            _logger.LogInformation("Table deleted: {TableId}", id);

            return Ok(ApiResponse<object>.Ok("تم حذف الطاولة بنجاح"));
        }
    }
}

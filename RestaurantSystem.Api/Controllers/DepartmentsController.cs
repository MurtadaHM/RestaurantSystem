using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Departments;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة الأقسام التشغيلية للمطعم (مطبخ، باربستا، حلويات، إلخ)
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(
            IDepartmentService departmentService,
            ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        // ──────────────────────────────────────────
        // GET /api/v1/departments
        // ──────────────────────────────────────────
        /// <summary>جلب قائمة بجميع الأقسام (للمدراء)</summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Fetching all departments.");
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(ApiResponse<IEnumerable<DepartmentResponseDto>>.Ok(departments));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/departments/active
        // ──────────────────────────────────────────
        /// <summary>جلب الأقسام الفعالة فقط (لشاشات الطلب)</summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentResponseDto>>>> GetActive()
        {
            var departments = await _departmentService.GetActiveDepartmentsAsync();
            return Ok(ApiResponse<IEnumerable<DepartmentResponseDto>>.Ok(departments));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/departments/{id}
        // ──────────────────────────────────────────
        /// <summary>جلب تفاصيل قسم محدد</summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> GetById(Guid id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return Ok(ApiResponse<DepartmentResponseDto>.Ok(department));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/departments
        // ──────────────────────────────────────────
        /// <summary>إنشاء قسم تشغيلي جديد (للآدمن فقط)</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> Create([FromBody] CreateDepartmentRequestDto request)
        {
            _logger.LogInformation("Creating a new department: {Name}", request.Name);
            var result = await _departmentService.CreateDepartmentAsync(request);
            return Ok(ApiResponse<DepartmentResponseDto>.Ok(result, "تم إنشاء القسم بنجاح"));
        }

        // ──────────────────────────────────────────
        // PUT /api/v1/departments/{id}
        // ──────────────────────────────────────────
        /// <summary>تحديث بيانات قسم</summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> Update(Guid id, [FromBody] UpdateDepartmentRequestDto request)
        {
            var result = await _departmentService.UpdateDepartmentAsync(id, request);
            return Ok(ApiResponse<DepartmentResponseDto>.Ok(result, "تم تحديث بيانات القسم"));
        }

        // ──────────────────────────────────────────
        // PATCH /api/v1/departments/{id}/status
        // ──────────────────────────────────────────
        /// <summary>تغيير حالة القسم (Active, Busy, Inactive)</summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromQuery] DepartmentStatus status)
        {
            await _departmentService.UpdateStatusAsync(id, status);
            return Ok(ApiResponse<bool>.Ok(true, $"تم تغيير حالة القسم إلى {status}"));
        }

        // ──────────────────────────────────────────
        // DELETE /api/v1/departments/{id}
        // ──────────────────────────────────────────
        /// <summary>حذف قسم (يجب أن لا يحتوي على أصناف منيو مرتبطة)</summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return Ok(ApiResponse<bool>.Ok(true, "تم حذف القسم بنجاح"));
        }
    }
}
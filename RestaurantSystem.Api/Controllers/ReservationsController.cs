using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Reservation;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة حجوزات الطاولات
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            IReservationService reservationService,
            ILogger<ReservationsController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// جلب كافة الحجوزات (للمدير)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Fetching all reservations");
            var result = await _reservationService.GetAllReservationsAsync();
            return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.Ok(result));
        }

        /// <summary>
        /// جلب حجوزات اليوم فقط (لموظف الاستقبال)
        /// </summary>
        [Authorize]
        [HttpGet("today")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponseDto>>>> GetToday()
        {
            var result = await _reservationService.GetTodayReservationsAsync();
            return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.Ok(result));
        }

        /// <summary>
        /// إضافة حجز جديد
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Create([FromBody] CreateReservationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating reservation for: {Customer}", request.CustomerName);
                var result = await _reservationService.CreateReservationAsync(request);
                return Ok(ApiResponse<ReservationResponseDto>.Ok(result, "تم تسجيل الحجز بنجاح"));
            }
            catch (Exception ex)
            {
                // هنا راح يرجع خطأ "تضارب المواعيد" اللي برمجناه بالـ Service
                _logger.LogWarning("Reservation failed: {Message}", ex.Message);
                return BadRequest(ApiResponse<ReservationResponseDto>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// تحديث حالة الحجز (مؤكد، ملغى، الخ)
        /// </summary>
        [Authorize]
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(Guid id, [FromQuery] ReservationStatus status)
        {
            await _reservationService.UpdateStatusAsync(id, status);
            return Ok(ApiResponse<object>.Ok(null, $"تم تحديث حالة الحجز إلى {status}"));
        }

        /// <summary>
        /// حذف حجز
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            await _reservationService.DeleteReservationAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "تم حذف الحجز بنجاح"));
        }
    }
}
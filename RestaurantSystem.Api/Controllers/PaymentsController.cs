using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Payments;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة المدفوعات
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب جميع المدفوعات — للمدير فقط
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponseDto>>>> GetAllPayments()
        {
            _logger.LogInformation("Fetching all payments");
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.Ok(payments));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب دفع محدد بالـ ID
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPayment(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetPayment called with empty ID");
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("معرف الدفع غير صحيح"));
            }

            _logger.LogInformation("Fetching payment: {PaymentId}", id);
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(payment));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/order/{orderId}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب الدفع الخاص بطلب معين
        /// </summary>
        [Authorize]
        [HttpGet("order/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPaymentByOrder(
            Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("GetPaymentByOrder called with empty orderId");
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Fetching payment for order: {OrderId}", orderId);
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(payment));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/status/{status}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب المدفوعات بحالة معينة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponseDto>>>> GetPaymentsByStatus(
            PaymentStatus status)
        {
            _logger.LogInformation("Fetching payments by status: {Status}", status);
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.Ok(payments));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/order/{orderId}/is-paid
        // ──────────────────────────────────────────
        /// <summary>
        /// التحقق من دفع طلب معين
        /// </summary>
        [Authorize]
        [HttpGet("order/{orderId:guid}/is-paid")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> IsOrderPaid(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("IsOrderPaid called with empty orderId");
                return BadRequest(ApiResponse<bool>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Checking if order is paid: {OrderId}", orderId);
            var isPaid = await _paymentService.IsOrderPaidAsync(orderId);
            return Ok(ApiResponse<bool>.Ok(isPaid));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/payments
        // ──────────────────────────────────────────
        /// <summary>
        /// إنشاء عملية دفع جديدة
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CreatePayment(
            [FromBody] CreatePaymentRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreatePayment called with null request");
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation(
                "Creating payment for order: {OrderId}, Method: {PaymentMethod}",
                request.OrderId, request.PaymentMethod);

            var result = await _paymentService.CreatePaymentAsync(request);

            _logger.LogInformation("Payment created: {PaymentId}", result.Id);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "تم إنشاء عملية الدفع بنجاح"));
        }

        // ──────────────────────────────────────────
        // PATCH /api/v1/payments/{id}/status
        // ──────────────────────────────────────────
        /// <summary>
        /// تحديث حالة الدفع
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> UpdatePaymentStatus(
            Guid id,
            [FromBody] UpdatePaymentStatusRequestDto request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdatePaymentStatus called with empty ID");
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("معرف الدفع غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdatePaymentStatus called with null request");
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation(
                "Updating payment {PaymentId} status to {NewStatus}",
                id, request.NewStatus);

            var result = await _paymentService.UpdatePaymentStatusAsync(id, request);

            _logger.LogInformation("Payment status updated: {PaymentId}", id);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "تم تحديث حالة الدفع بنجاح"));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/payments/{id}/refund
        // ──────────────────────────────────────────
        /// <summary>
        /// استرداد دفعة مكتملة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id:guid}/refund")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> RefundPayment(
            Guid id,
            [FromQuery] string? notes = null)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("RefundPayment called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف الدفع غير صحيح"));
            }

            _logger.LogInformation("Refunding payment: {PaymentId}", id);
            var result = await _paymentService.RefundPaymentAsync(id, notes);

            if (!result)
            {
                _logger.LogWarning("Refund failed for payment: {PaymentId}", id);
                return BadRequest(ApiResponse<object>.Fail("الدفع غير موجود"));
            }

            _logger.LogInformation("Payment refunded: {PaymentId}", id);
            return Ok(ApiResponse<object>.Ok("تم استرداد الدفع بنجاح"));
        }
    }
}

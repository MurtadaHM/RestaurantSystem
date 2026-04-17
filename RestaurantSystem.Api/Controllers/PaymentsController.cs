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
    /// إدارة عمليات الدفع والنظام المالي للمطعم
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
        /// جلب جميع السجلات المالية (للمدراء فقط)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponseDto>>>> GetAllPayments()
        {
            _logger.LogInformation("Fetching all payments from the system.");
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.Ok(payments));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب تفاصيل عملية دفع محددة
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPayment(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("معرف الدفع غير صحيح"));

            _logger.LogInformation("Fetching payment details for ID: {PaymentId}", id);
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(payment));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/payments/order/{orderId}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب الدفع الخاص بطلب معين (Order)
        /// </summary>
        [Authorize]
        [HttpGet("order/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPaymentByOrder(Guid orderId)
        {
            if (orderId == Guid.Empty)
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("معرف الطلب غير صحيح"));

            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return Ok(ApiResponse<PaymentResponseDto>.Ok(payment));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/payments
        // ──────────────────────────────────────────
        /// <summary>
        /// تسجيل عملية دفع جديدة لطلب
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            if (request == null)
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("بيانات الدفع مطلوبة"));

            _logger.LogInformation("Processing new payment for Order: {OrderId}", request.OrderId);
            var result = await _paymentService.CreatePaymentAsync(request);

            return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "تم تسجيل عملية الدفع بنجاح"));
        }

        // ──────────────────────────────────────────
        // PATCH /api/v1/payments/{id}/status
        // ──────────────────────────────────────────
        /// <summary>
        /// تحديث حالة الدفع (تأكيد الدفع يحول الطاولة إلى متاحة تلقائياً)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusRequestDto request)
        {
            if (id == Guid.Empty || request == null)
                return BadRequest(ApiResponse<PaymentResponseDto>.Fail("البيانات المرسلة غير مكتملة"));

            _logger.LogInformation("Updating status for payment {PaymentId} to {NewStatus}", id, request.NewStatus);
            var result = await _paymentService.UpdatePaymentStatusAsync(id, request);

            return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "تم تحديث حالة الدفع وتحديث حالة النظام المرتبطة"));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/payments/{id}/refund
        // ──────────────────────────────────────────
        /// <summary>
        /// استرداد مبلغ عملية دفع مكتملة (للمدراء فقط)
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id:guid}/refund")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<object>>> RefundPayment(Guid id, [FromQuery] string? notes = null)
        {
            if (id == Guid.Empty)
                return BadRequest(ApiResponse<object>.Fail("معرف الدفع غير صحيح"));

            _logger.LogWarning("Refunding payment: {PaymentId}", id);

            // الميدل وير سيتكفل بالأخطاء مثل 404 أو 409 بناءً على الـ Exceptions الملقاة
            await _paymentService.RefundPaymentAsync(id, notes);

            _logger.LogInformation("Payment {PaymentId} refunded successfully.", id);
            return Ok(ApiResponse<object>.Ok(null, "تم استرداد الدفع بنجاح بنجاح"));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    /// <summary>
    /// إدارة الطلبات
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب جميع الطلبات — للمدير والمدير فقط
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAllOrders()
        {
            _logger.LogInformation("Fetching all orders");
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders/pending
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب الطلبات المعلقة فقط
        /// </summary>
        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpGet("pending")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetPendingOrders()
        {
            _logger.LogInformation("Fetching pending orders");
            var orders = await _orderService.GetPendingOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب طلب محدد بالـ ID
        /// </summary>
        [Authorize]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrder(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetOrder called with empty ID");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Fetching order: {OrderId}", id);
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders/user/{userId}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب طلبات مستخدم معين
        /// </summary>
        [Authorize]
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrdersByUser(
            string userId)
        {
            // ✅ المستخدم العادي يشوف طلباته فقط — المدير يشوف أي مستخدم
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = User.FindFirstValue(ClaimTypes.Role);

            var isAdminOrManager = currentRole is "Admin" or "Manager";

            if (!isAdminOrManager && currentUserId != userId)
            {
                _logger.LogWarning(
                    "User {CurrentUser} tried to access orders of {TargetUser}",
                    currentUserId, userId);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.Fail("ليس لديك صلاحية لعرض طلبات مستخدم آخر"));
            }

            _logger.LogInformation("Fetching orders for user: {UserId}", userId);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders/table/{tableId}
        // ──────────────────────────────────────────
        /// <summary>
        /// جلب طلبات طاولة معينة
        /// </summary>
        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpGet("table/{tableId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrdersByTable(
            Guid tableId)
        {
            if (tableId == Guid.Empty)
            {
                _logger.LogWarning("GetOrdersByTable called with empty tableId");
                return BadRequest(ApiResponse<IEnumerable<OrderResponseDto>>.Fail("معرف الطاولة غير صحيح"));
            }

            _logger.LogInformation("Fetching orders for table: {TableId}", tableId);
            var orders = await _orderService.GetOrdersByTableIdAsync(tableId);
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/orders
        // ──────────────────────────────────────────
        /// <summary>
        /// إنشاء طلب جديد
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder(
            [FromBody] CreateOrderRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateOrder called with null request");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("البيانات مطلوبة"));
            }

            // ✅ نأخذ الـ UserId من الـ JWT Token مباشرة — أأمن من أن يرسله العميل
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                request.UserId = userId;
            }

            // ✅ تم التغيير من request.Type إلى request.OrderType
            _logger.LogInformation("Creating order for user: {UserId}, Type: {OrderType}", request.UserId, request.OrderType);

            var result = await _orderService.CreateOrderAsync(request);

            _logger.LogInformation("Order created: {OrderId}", result.Id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تم إنشاء الطلب بنجاح"));
        }

        // ──────────────────────────────────────────
        // PATCH /api/v1/orders/{id}/status
        // ──────────────────────────────────────────
        /// <summary>
        /// تحديث حالة طلب معين
        /// </summary>
        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrderStatus(
            Guid id,
            [FromBody] UpdateOrderStatusRequestDto request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateOrderStatus called with empty ID");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف الطلب غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdateOrderStatus called with null request");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("البيانات مطلوبة"));
            }

            // ✅ نضع الـ ID من الـ URL في الـ request دائماً
            request.OrderId = id;

            _logger.LogInformation(
                "Updating order {OrderId} status to {NewStatus}",
                id, request.NewStatus);

            var result = await _orderService.UpdateOrderStatusAsync(id, request);

            _logger.LogInformation("Order status updated: {OrderId}", id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تم تحديث حالة الطلب بنجاح"));
        }

        // ──────────────────────────────────────────
        // PUT /api/v1/orders/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// تعديل طلب كامل
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrder(
            Guid id,
            [FromBody] CreateOrderRequestDto request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateOrder called with empty ID");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف الطلب غير صحيح"));
            }

            if (request == null)
            {
                _logger.LogWarning("UpdateOrder called with null request");
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("البيانات مطلوبة"));
            }

            _logger.LogInformation("Updating order: {OrderId}", id);
            var result = await _orderService.UpdateOrderAsync(id, request);

            _logger.LogInformation("Order updated: {OrderId}", id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تم تعديل الطلب بنجاح"));
        }

        // ──────────────────────────────────────────
        // POST /api/v1/orders/{id}/cancel
        // ──────────────────────────────────────────
        /// <summary>
        /// إلغاء طلب — فقط لو حالته Pending
        /// </summary>
        [Authorize]
        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> CancelOrder(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("CancelOrder called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Cancelling order: {OrderId}", id);
            var result = await _orderService.CancelOrderAsync(id);

            if (!result)
            {
                _logger.LogWarning("Cancel failed for order: {OrderId}", id);
                return BadRequest(ApiResponse<object>.Fail("لا يمكن إلغاء الطلب"));
            }

            _logger.LogInformation("Order cancelled: {OrderId}", id);
            return Ok(ApiResponse<object>.Ok("تم إلغاء الطلب بنجاح"));
        }

        // ──────────────────────────────────────────
        // DELETE /api/v1/orders/{id}
        // ──────────────────────────────────────────
        /// <summary>
        /// حذف طلب — للمدير فقط
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOrder(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteOrder called with empty ID");
                return BadRequest(ApiResponse<object>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Deleting order: {OrderId}", id);
            var result = await _orderService.DeleteOrderAsync(id);

            if (!result)
            {
                _logger.LogWarning("Delete failed for order: {OrderId}", id);
                return BadRequest(ApiResponse<object>.Fail("الطلب غير موجود"));
            }

            _logger.LogInformation("Order deleted: {OrderId}", id);
            return Ok(ApiResponse<object>.Ok("تم حذف الطلب بنجاح"));
        }

        // ──────────────────────────────────────────
        // GET /api/v1/orders/{id}/total
        // ──────────────────────────────────────────
        /// <summary>
        /// حساب المجموع الكلي للطلب
        /// </summary>
        [Authorize]
        [HttpGet("{id:guid}/total")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetOrderTotal(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetOrderTotal called with empty ID");
                return BadRequest(ApiResponse<decimal>.Fail("معرف الطلب غير صحيح"));
            }

            _logger.LogInformation("Calculating total for order: {OrderId}", id);
            var total = await _orderService.CalculateOrderTotalAsync(id);
            return Ok(ApiResponse<decimal>.Ok(total));
        }
    }
}

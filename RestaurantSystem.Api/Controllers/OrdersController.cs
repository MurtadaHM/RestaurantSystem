using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Manager,Cashier,Waiter")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderResponseDto>.Fail("الطلب غير موجود"));

            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }

        [Authorize]
        [HttpGet("number/{orderNumber:int}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderByNumber(int orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (order == null)
                return NotFound(ApiResponse<OrderResponseDto>.Fail($"الطلب رقم {orderNumber} غير موجود"));

            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetUserOrders(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out Guid userId))
                request.UserId = userId;

            try
            {
                var result = await _orderService.CreateOrderAsync(request);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = result.Id },
                    ApiResponse<OrderResponseDto>.Ok(result, "تم إنشاء الطلب بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Manager,Cashier,Waiter")]
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequestDto request)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, request);
                return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تم تحديث الحالة بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
            }
        }

        // NEW: جلب تقدم الأقسام داخل الطلب
        [Authorize]
        [HttpGet("{id:guid}/department-progress")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDepartmentProgressDto>>>> GetOrderDepartmentProgress(Guid id)
        {
            try
            {
                var result = await _orderService.GetOrderDepartmentProgressAsync(id);
                return Ok(ApiResponse<IEnumerable<OrderDepartmentProgressDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<OrderDepartmentProgressDto>>.Fail(ex.Message));
            }
        }

        // NEW: تحديث حالة قسم داخل الطلب
        [Authorize(Roles = "Admin,Manager,Cashier,Waiter,Chef,Barista")]
        [HttpPatch("{id:guid}/department-status")]
        public async Task<ActionResult<ApiResponse<OrderDepartmentProgressDto>>> UpdateOrderDepartmentStatus(
            Guid id,
            [FromBody] UpdateOrderDepartmentStatusRequestDto request)
        {
            try
            {
                var result = await _orderService.UpdateOrderDepartmentStatusAsync(id, request);
                return Ok(ApiResponse<OrderDepartmentProgressDto>.Ok(result, "تم تحديث حالة القسم بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDepartmentProgressDto>.Fail(ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id:guid}/push-external")]
        public async Task<ActionResult<ApiResponse<bool>>> PushToDelivery(Guid id)
        {
            var success = await _orderService.PushOrderToExternalDeliveryAsync(id);
            if (!success)
                return BadRequest(ApiResponse<bool>.Fail("فشل إرسال الطلب، تأكد من صحة العنوان أو المحاولة لاحقاً"));

            return Ok(ApiResponse<bool>.Ok(true, "تم إرسال الطلب لشركة التوصيل بنجاح"));
        }

        // NEW ENDPOINT: push to Sendy (internal store integration)
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id:guid}/push-to-sendy")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> PushToSendy(Guid id)
        {
            try
            {
                var result = await _orderService.PushOrderToSendyAsync(id);
                return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تم إرسال الطلب إلى Sendy بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("{id:guid}/sync-delivery")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> SyncDeliveryStatus(Guid id)
        {
            var result = await _orderService.SyncExternalStatusAsync(id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تمت مزامنة البيانات مع شركة التوصيل"));
        }

        [Authorize]
        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<ApiResponse<object>>> CancelOrder(Guid id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);

                return result
                    ? Ok(ApiResponse<object>.Ok(null, "تم إلغاء الطلب"))
                    : BadRequest(ApiResponse<object>.Fail("لا يمكن إلغاء الطلب في حالته الحالية"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<object>>> GetOrderStats()
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            var stats = new
            {
                Total = allOrders.Count(),
                TodayRevenue = allOrders
                    .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date && o.Status == "Completed")
                    .Sum(o => o.TotalAmount),
                ByStatus = allOrders
                    .GroupBy(o => o.Status)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(ApiResponse<object>.Ok(stats));
        }
    }
}
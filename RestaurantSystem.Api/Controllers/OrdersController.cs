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

        // 1. جلب كل الطلبات
        // تم السماح للكاشير والويتر حتى يقدرون يعرضون الطلبات في الواجهة
        [Authorize(Roles = "Admin,Manager,Cashier,Waiter")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // 2. جلب تفاصيل طلب بالـ GUID
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderResponseDto>.Fail("الطلب غير موجود"));

            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }

        // 3. البحث برقم الطلب البسيط
        [Authorize]
        [HttpGet("number/{orderNumber:int}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderByNumber(int orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (order == null)
                return NotFound(ApiResponse<OrderResponseDto>.Fail($"الطلب رقم {orderNumber} غير موجود"));

            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }

        // 4. جلب طلبات مستخدم معين
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetUserOrders(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
        }

        // 5. إنشاء طلب جديد
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

        // 6. تحديث الحالة
        [Authorize(Roles = "Admin,Manager,Staff,Cashier,Waiter")]
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

        // 7. إرسال الطلب لشركة التوصيل يدوياً
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id:guid}/push-external")]
        public async Task<ActionResult<ApiResponse<bool>>> PushToDelivery(Guid id)
        {
            var success = await _orderService.PushOrderToExternalDeliveryAsync(id);
            if (!success)
                return BadRequest(ApiResponse<bool>.Fail("فشل إرسال الطلب، تأكد من صحة العنوان أو المحاولة لاحقاً"));

            return Ok(ApiResponse<bool>.Ok(true, "تم إرسال الطلب لشركة التوصيل بنجاح"));
        }

        // 8. مزامنة فورية لحالة التوصيل
        [Authorize]
        [HttpPost("{id:guid}/sync-delivery")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> SyncDeliveryStatus(Guid id)
        {
            var result = await _orderService.SyncExternalStatusAsync(id);
            return Ok(ApiResponse<OrderResponseDto>.Ok(result, "تمت مزامنة البيانات مع شركة التوصيل"));
        }

        // 9. إلغاء الطلب
        [Authorize]
        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<ApiResponse<object>>> CancelOrder(Guid id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            return result
                ? Ok(ApiResponse<object>.Ok(null, "تم إلغاء الطلب"))
                : BadRequest(ApiResponse<object>.Fail("لا يمكن إلغاء الطلب في حالته الحالية"));
        }

        // 10. إحصائيات لوحة التحكم
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
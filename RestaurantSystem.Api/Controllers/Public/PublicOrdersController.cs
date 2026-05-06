using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.DTOs.PublicOrders;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/orders")]
    [Produces("application/json")]
    [Tags("Public Orders")]
    public class PublicOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public PublicOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("table/{code}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateTableOrder(
            string code,
            [FromBody] CreatePublicTableOrderRequestDto request)
        {
            try
            {
                var order = await _orderService.CreatePublicTableOrderAsync(code, request);

                return Ok(ApiResponse<OrderResponseDto>.Ok(
                    order,
                    "تم إنشاء الطلب بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
            }
        }
    }
}
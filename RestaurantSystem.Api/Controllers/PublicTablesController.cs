using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Integrations.Team6;
using RestaurantSystem.Application.DTOs.Tables;
using RestaurantSystem.Application.Configurations;
namespace RestaurantSystem.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/tables")]
    [Produces("application/json")]
    [Tags("Public Tables")]
    public class PublicTablesController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly Team6IntegrationSettings _team6Settings;

        public PublicTablesController(
            ITableRepository tableRepository,
            IOptions<Team6IntegrationSettings> team6Settings)
        {
            _tableRepository = tableRepository;
            _team6Settings = team6Settings.Value;
        }

        /// <summary>
        /// جلب بيانات الطاولة بواسطة QR Code
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<ApiResponse<TableResponseDto>>> GetByCode(string code)
        {
            var table = await _tableRepository.GetByCodeAsync(code);

            if (table == null)
            {
                return NotFound(ApiResponse<TableResponseDto>.Fail("الطاولة غير موجودة"));
            }

            if (!table.IsActive)
            {
                return BadRequest(ApiResponse<TableResponseDto>.Fail("الطاولة غير فعالة"));
            }

            if (!table.IsOrderingEnabled)
            {
                return BadRequest(ApiResponse<TableResponseDto>.Fail("الطلب الإلكتروني غير متاح لهذه الطاولة"));
            }

            var response = new TableResponseDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Code = table.Code,
                Capacity = table.Capacity,
                Location = table.Location,
                Zone = table.Zone,
                FloorNumber = table.FloorNumber,
                Status = table.Status.ToString(),
                IsActive = table.IsActive,
                IsOrderingEnabled = table.IsOrderingEnabled,
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt ?? table.CreatedAt,
                ActiveOrdersCount = table.Orders?.Count(o => !o.IsDeleted) ?? 0
            };

            return Ok(ApiResponse<TableResponseDto>.Ok(response));
        }

        /// <summary>
        /// Endpoint خاص بتكامل Team 6
        /// يرجع restaurantId الخارجي + tableId الحالي مع بيانات الطاولة
        /// </summary>
        [HttpGet("team6/session/{code}")]
        public async Task<ActionResult<ApiResponse<Team6TableSessionResponseDto>>> GetTeam6TableSession(string code)
        {
            if (!_team6Settings.Enabled)
            {
                return BadRequest(ApiResponse<Team6TableSessionResponseDto>.Fail("تكامل Team 6 غير مفعل حالياً"));
            }

            var table = await _tableRepository.GetByCodeAsync(code);

            if (table == null)
            {
                return NotFound(ApiResponse<Team6TableSessionResponseDto>.Fail("الطاولة غير موجودة"));
            }

            var response = new Team6TableSessionResponseDto
            {
                RestaurantId = _team6Settings.RestaurantId,
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Code = table.Code,
                Location = table.Location,
                Zone = table.Zone,
                FloorNumber = table.FloorNumber,
                IsActive = table.IsActive,
                IsOrderingEnabled = table.IsOrderingEnabled
            };

            return Ok(ApiResponse<Team6TableSessionResponseDto>.Ok(response));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RestaurantSystem.Api.Common;
using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Application.Services.Interfaces;

namespace RestaurantSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("محاولة تسجيل دخول: {Email}", request.Email);
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("فشل تسجيل الدخول: {Email}", request.Email);
                return Unauthorized(ApiResponse<AuthResponseDto>.Fail(result.Message));
            }

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "تم تسجيل الدخول بنجاح"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("محاولة إنشاء حساب جديد: {Email}", request.Email);
            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Message));
            }

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "تم إنشاء الحساب بنجاح"));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _authService.LogoutAsync(userId);
            _logger.LogInformation("تم تسجيل الخروج للمستخدم: {UserId}", userId);

            return Ok(ApiResponse<object>.Ok(null, "تم تسجيل الخروج بنجاح"));
        }

        [Authorize]
        [HttpGet("me")]
        public ActionResult<ApiResponse<UserAuthDto>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // ملاحظة لمرتضى: تأكد أنك أضفت FirstName و LastName في الـ Claims عند توليد الـ Token
            var user = new UserAuthDto
            {
                Id = Guid.Parse(userId),
                Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
                FirstName = User.FindFirstValue("FirstName") ?? "User",
                LastName = User.FindFirstValue("LastName") ?? ""
            };

            return Ok(ApiResponse<UserAuthDto>.Ok(user));
        }
    }
}
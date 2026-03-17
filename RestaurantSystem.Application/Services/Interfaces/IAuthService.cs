using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<bool> ValidateTokenAsync(string token);
        Task LogoutAsync(string userId);
    }
}

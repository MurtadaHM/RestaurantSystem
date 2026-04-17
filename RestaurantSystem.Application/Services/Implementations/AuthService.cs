using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Application.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "بيانات المستخدم غير صحيحة"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "الحساب غير مفعل، يرجى مراجعة الإدارة"
                };
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            var expiryInMinutes = double.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "1440");

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم تسجيل الدخول بنجاح",
                Token = token,
                User = MapToUserAuthDto(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryInMinutes)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "البريد الإلكتروني مستخدم بالفعل"
                };
            }

            if (await _userRepository.ExistsByPhoneAsync(request.PhoneNumber))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "رقم الهاتف مستخدم بالفعل"
                };
            }

            var user = new User
            {
                Email = request.Email.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            var token = GenerateJwtToken(user);
            var expiryInMinutes = double.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "1440");

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم إنشاء الحساب بنجاح",
                Token = token,
                User = MapToUserAuthDto(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryInMinutes)
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await Task.FromResult(true);
        }

        public Task LogoutAsync(string userId) => Task.CompletedTask;

        private UserAuthDto MapToUserAuthDto(User user)
        {
            return new UserAuthDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? "User",
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = user.Role.ToString(),
                ProfileImageUrl = user.ProfileImageUrl ?? string.Empty
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                         ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var expiryInMinutes = double.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "1440");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
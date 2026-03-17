using RestaurantSystem.Application.DTOs.Auth;
using RestaurantSystem.Domain.Entities;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Application.Contracts.Repositories; // ✅ استدعاء الـ Repositories

namespace RestaurantSystem.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository; // ✅ استخدام Repository بدلاً من DbContext
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // ✅ استخدام الـ Repository للبحث عن المستخدم
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "بيانات المستخدم غير صحيحة"
                };
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم تسجيل الدخول بنجاح",
                Token = token,
                User = MapToUserAuthDto(user),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // ✅ استخدام الـ Repository للتحقق من وجود الإيميل
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "البريد الإلكتروني مستخدم بالفعل"
                };
            }

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            // ✅ إضافة المستخدم باستخدام الـ Repository
            await _userRepository.AddAsync(user);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم إنشاء الحساب بنجاح",
                Token = token,
                User = MapToUserAuthDto(user),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        private UserAuthDto MapToUserAuthDto(User user)
        {
            return new UserAuthDto
            {
                Id = user.Id,
                FirstName = user.FirstName!, // الـ ! تخبر المترجم: "أنا أضمن لك أنها ليست null"
                LastName = user.LastName!,
                Email = user.Email!,
                // بما أن PhoneNumber قد يكون نول في الإعدادات، يفضل معالجته ببديل
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = user.Role.ToString()
            };
        }

        private string GenerateJwtToken(User user)
        {
            // ✅ الحل: التأكد من أن المفتاح موجود وإلا رمي استثناء واضح
            var jwtKey = _configuration["Jwt:Key"]
                         ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            // أضف ! إذا كان المترجم يحذرك من user.Email أيضاً
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"], // يفضل معالجتها بنفس الطريقة أو إضافة !
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<bool> ValidateTokenAsync(string token) => throw new NotImplementedException();
        public Task LogoutAsync(string userId) => Task.CompletedTask;
    }
}

using Api.DTO;
using ApiDomain;
using ApiDomain.Models;
using ApiDomain.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ApiListContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ITokenService tokenService, ApiListContext context, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequestDto request) // Внедрение контекста БД)
        {
            // Проверка, что email не занят
            if (await _context.AuthUser.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email уже используется");
            // Валидация пароля (пример: минимум 8 символов)
            if (request.Password.Length < 8)
                return BadRequest("Пароль должен содержать минимум 8 символов");

            // Создание Customer
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                Email = request.Email,
                NameCustomer = request.Name,
                DateOfBirthday = request.transformBirthData(request.BirthData) 
            };

            // Создание AuthUser
            var authUser = new AuthUser
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.CustomerId,
                Email = request.Email
            };
            authUser.HashPassword(request.Password);


            // Сохранение в БД
            await _context.Customer.AddAsync(customer);
            await _context.AuthUser.AddAsync(authUser);
            await _context.SaveChangesAsync();

            // Генерация токена
            var accessToken = _tokenService.GenerateToken(authUser.Id, authUser.Email, authUser.Role);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                AuthUserId = authUser.Id
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            }); ;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestLoginDto request)
        {
            // Поиск пользователя в БД
            var authUser = await _context.AuthUser.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (authUser == null || !authUser.VerifyPassword(request.Password))
                return BadRequest("Неверный email или пароль");

            // Поиск связанного Customer
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (customer == null)
                return BadRequest("Профиль покупателя не найден");

            // Генерация токена
            var token = _tokenService.GenerateToken(authUser.Id, authUser.Email, authUser.Role);
            return Ok(new { token });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody]RefreshTokenRequestDto refToken)
        {
            var token = await _context.RefreshTokens
                .Include(r => r.AuthUser)
                .FirstOrDefaultAsync(r => r.Token == refToken.Token);
            if (token == null || token.IsExpired)
                return Unauthorized(new { error = "Токен невалиден или истёк." });
            
            var authUser = await _context.AuthUser.FindAsync(token.AuthUserId);
            if (authUser == null)
                return Unauthorized(new { error = "Пользователь не найден." });
            // Генерация нового Access Token
            var newAccessToken = _tokenService.GenerateToken(token.AuthUser.Id, token.AuthUser.Email, "User");

            // Обновление Refresh Token (опционально)
            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                AuthUserId = token.AuthUser.Id
            };

            _context.RefreshTokens.Remove(token);
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        /*[HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(request.Token);
                var email = principal.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Невалидный токен: отсутствует email.");

                var user = await _context.AuthUser
                .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("Пользователь не найден.");
                }

                var newToken = _tokenService.GenerateToken(Guid.NewGuid(), email, user.Role);
                return Ok(new { token = newToken });
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest($"Ошибка валидации токена: {ex.Message}");
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!)),
                ValidateLifetime = false // пропускаем срок действия
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return principal;
        }*/
    }
}


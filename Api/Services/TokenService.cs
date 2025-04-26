using ApiDomain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public RefreshToken GenerateRefreshToken(Guid authUserId)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Expires = DateTime.UtcNow.AddDays(7),
                AuthUserId = authUserId
            };
        }

        public string GenerateToken(Guid userId, string email, string role)
        {
            var claims = new[] //данные пользователя
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), //Sub (идентификатор пользователя) из БД
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Jti (уникальный идентификатор токена) - по новому айди
            new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //алгоритм

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"], //издатель
                audience: _config["JwtSettings:Audience"], //аудитория
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

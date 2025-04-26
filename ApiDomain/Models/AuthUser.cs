using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ApiDomain.Models
{
    public class AuthUser //модель пользователя
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; } // Ссылка на Customer
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // по умолчанию обычный пользователь
        public void HashPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
        }

        // Проверка пароля
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, PasswordHash);
        }
    }
}

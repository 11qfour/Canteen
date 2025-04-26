using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class AuthRequestLoginDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
        public string Password { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Токен обязателен")]
        public string Token { get; set; } = string.Empty;
    }
}

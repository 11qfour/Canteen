using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Api.DTO
{
    public class AuthRequestDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [RegularExpression(
        @"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4}\s([01][0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "Формат даты: DD.MM.YYYY HH:MM"
        )]
        public string BirthData { get; set; } = string.Empty;
        //формат который будет приходить
        //01.01.2000 00:00
        //формат в которые надо преобразовать
        //2025-03-22T13:03:53.790Z

        public DateTime transformBirthData(string birthData)//преобзование в формат
        {
            if (DateTime.TryParseExact(
                birthData,
                "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return parsedDate; 
            }
            else
            {
                throw new ArgumentException("Некорректный формат даты. Ожидается DD.MM.YYYY HH:MM");
            }
        }
    }
}

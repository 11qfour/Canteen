using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CustomerCreateDto
    {
        [Required(ErrorMessage = "Name Customer is required")]
        public string NameCustomer { get; set; } = string.Empty;
        [Required(ErrorMessage = "Date Of Birth is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
    }
}

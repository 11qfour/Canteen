using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CustomerUpdateDto
    {
        [Required(ErrorMessage = "Name Customer is required")]
        public string NameCustomer { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
    }
}

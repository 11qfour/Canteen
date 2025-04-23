using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CustomerDto
    {
        [Required(ErrorMessage = "Customer Id is required")]
        public Guid CustomerId { get; set; }
        [Required(ErrorMessage = "Name Customer is required")]
        public string NameCustomer { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public List<OrderDto> orders { get; set; } = new();
    }
}

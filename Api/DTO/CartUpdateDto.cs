using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CartUpdateDto
    {
        [Required(ErrorMessage = "Customer Id is required.")]
        public Guid CustomerId { get; set; }
        [Required(ErrorMessage = "TotalPrice is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }
        [Required(ErrorMessage = "Cart details are required.")]
        [MinLength(1, ErrorMessage = "Cart must contain at least one item.")]
        public List<CartDetailsDto> CartDetails { get; set; } = new();
    }
}

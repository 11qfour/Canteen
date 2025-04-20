using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class OrderDto
    {
        [Required(ErrorMessage = "Order Id is required.")]
        public Guid OrderId { get; set; }
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Confirmed|InCooking|Ready|Issued|NotPaid|Canceled)$", ErrorMessage = "Invalid status.")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Total Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Customer Name is required.")]
        public string CustomerName { get; set; } // Имя клиента
        [Required(ErrorMessage = "Order Details are required.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderDetailsDto> OrderDetails { get; set; } = new();
    }
}

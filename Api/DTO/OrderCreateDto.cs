using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Customer Id is required.")]
        public Guid CustomerId { get; set; } // Обязательно, кто делает заказ

        [Required(ErrorMessage = "Total Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; } // Итоговая сумма
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty; // Адрес доставки
        [Required(ErrorMessage = "Order Details are required.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderDetailsDto> OrderDetails { get; set; } = new();
    }
}

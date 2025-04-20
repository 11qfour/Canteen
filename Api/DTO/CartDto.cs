using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CartDto
    {
        [Required(ErrorMessage = "Cart Id is required.")]
        public Guid CartId { get; set; }  // Уникальный идентификатор корзины
        [Required(ErrorMessage = "Customer Name is required.")]
        public string CustomerName { get; set; } = string.Empty;  // Имя владельца
        [Required(ErrorMessage = "Total Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }  // Итоговая сумма
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Active|Ordered|Cancelled)$", ErrorMessage = "Invalid status.")]
        public string Status { get; set; }  // Статус корзины
        [Required(ErrorMessage = "Cart details are required.")]
        [MinLength(1, ErrorMessage = "Cart must contain at least one item.")]
        public List<CartDetailsDto> CartDetails { get; set; } = new();
    }
}

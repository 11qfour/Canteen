namespace Api.DTO
{
    public class CartDto
    {
        public Guid CartId { get; set; }  // Уникальный идентификатор корзины
        public string CustomerName { get; set; } = string.Empty;  // Имя владельца
        public decimal TotalPrice { get; set; }  // Итоговая сумма
        public string Status { get; set; }  // Статус корзины
        public List<CartDetailsDto> CartDetails { get; set; } = new();
    }
}

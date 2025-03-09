namespace Api.DTO
{
    public class CartDetailsDto
    {
        public Guid DishId { get; set; }  // ID блюда
        public string DishName { get; set; } = string.Empty;  // Название блюда
        public int Quantity { get; set; }  // Количество
        public decimal PriceUnit { get; set; }  // Цена за единицу
    }
}

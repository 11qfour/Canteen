namespace Api.DTO
{
    public class OrderCreateDto
    {
        public Guid CustomerId { get; set; } // Обязательно, кто делает заказ
        public decimal TotalPrice { get; set; } // Итоговая сумма
        public string Address { get; set; } = string.Empty; // Адрес доставки
        public Guid EmployeeId { get; set; } // Кто оформил заказ
    }
}

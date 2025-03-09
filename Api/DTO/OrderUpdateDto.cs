using ApiDomain.Enums;

namespace Api.DTO
{
    public class OrderUpdateDto
    {
        public OrderStatus Status { get; set; } // Можно менять только статус заказа
    }
}

namespace Api.DTO
{
    public class CartCreateDto
    {
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

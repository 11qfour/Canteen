namespace Api.DTO
{
    public class CartUpdateDto
    {
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

namespace Api.DTO
{
    public class DishDto
    {
        public Guid DishId { get; set; }
        public string DishName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } // Название категории
    }
}

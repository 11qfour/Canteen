namespace Api.DTO
{
    public class DishCreateDto
    {
        public string DishName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; } // В какой категории блюдо?
        public int CookingTime { get; set; } // Время приготовления
    }
}

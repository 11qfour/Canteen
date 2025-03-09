namespace Api.DTO
{
    public class DishUpdateDto
    {
        public string DishName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CookingTime { get; set; }
    }
}

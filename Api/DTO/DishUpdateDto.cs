using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class DishUpdateDto
    {
        [Required(ErrorMessage = "Dish Name is required")]
        public string DishName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Cooking Time  is required")]
        [Range(1, 45, ErrorMessage = "CookingTime must be greater than 1 and less than 45.")]
        public int CookingTime { get; set; }
    }
}

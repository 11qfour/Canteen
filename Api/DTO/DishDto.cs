using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class DishDto
    {
        [Required(ErrorMessage = "Dish Id is required")]
        public Guid DishId { get; set; }
        [Required(ErrorMessage = "Dish Name is required")]
        public string DishName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Dish Name is required")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; } = string.Empty;// Название категории
    }
}

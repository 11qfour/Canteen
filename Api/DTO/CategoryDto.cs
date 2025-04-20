using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Name Category is required")]
        public string NameCategory { get; set; }
    }
}

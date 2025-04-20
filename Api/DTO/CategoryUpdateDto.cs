using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Name Category is required")]
        public string NameCategory { get; set; } = string.Empty;
    }
}

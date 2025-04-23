using ApiDomain.Models;
using ApiDomain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class OrderDetailsDto
    {
        [Required(ErrorMessage = "Dish ID is required.")]
        public Guid DishId { get; set; }  // ID блюда

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }  // Количество

        [Required(ErrorMessage = "PriceUnit is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price Unit must be greater than 0.")]
        public decimal PriceUnit { get; set; }  // Цена за единицу
    }
}

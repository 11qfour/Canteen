using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using ApiDomain.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiDomain
{
    public class Dish
    {
        public Guid DishId { get; set; }
        public string  DishName{ get; set; } = string.Empty;
        public string Description { get; set; }= string.Empty;
       
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price{ get; set; }
        // Внешний ключ для связи с Category
        public Guid? CategoryId { get; set; }
        //навигационное свойство
        public Category? Category { get; set; }

        public int CookingTime{ get; set; }

        // Связь с деталями корзины
        public ICollection<CartDetails> CartDetails { get; set; } = new List<CartDetails>();
        // Связь с деталями заказа
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
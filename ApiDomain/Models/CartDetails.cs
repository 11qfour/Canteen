using ApiDomain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDomain
{
public class CartDetails
    {
        public Guid CartDetailsId { get; set; }
        // Внешний ключ на корзину (одна корзина -> много деталей)
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        // Внешний ключ на блюдо (одно блюдо -> может быть в нескольких корзинах)
        public Guid DishId { get; set; }
        public Dish Dish { get; set; } = null!;
        public int Quantity { get; set; }

        // Цена за единицу блюда (может отличаться от Dish.Price, например, со скидкой)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnit { get; set; }
    }
}
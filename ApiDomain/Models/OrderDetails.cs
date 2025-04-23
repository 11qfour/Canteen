using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Models
{
    public class OrderDetails
    {
        public Guid OrderDetailsId { get; set; }
        //внешний ключ на Order (в одном заказе может быть несколько блюд)
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        //внешний ключ на Dish (в разных заказах может быть одно блюдо)
        public Guid DishId { get; set; }
        public Dish Dish { get; set; } = null!;
        public int Quantity {get; set;}
        // Цена за единицу блюда (может отличаться от Dish.Price, например, со скидкой)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnit { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Для контроля параллелизма
    }
}

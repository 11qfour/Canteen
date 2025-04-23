using ApiDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Models
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; set; }
        // Навигационное свойство для связи с пользователем
        public Customer Customer { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        // Новый статус корзины
        public CartStatus Status { get; set; } = CartStatus.Active;
        // Навигационное свойство для связи с деталями корзины
        public ICollection<CartDetails> CartDetails { get; set; } = new List<CartDetails>();

        // Временные метки для статусов
        public DateTime? DateActivated { get; private set; }
        public DateTime? DateOrdered { get; private set; }
        public DateTime? DateCanceled { get; private set; }

        // Метод для обновления временных меток
        public void UpdateStatusTimestamp(CartStatus status)
        {
            switch (status)
            {
                case CartStatus.Active:
                    DateActivated = DateTime.UtcNow;
                    break;
                case CartStatus.Ordered:
                    DateOrdered = DateTime.UtcNow;
                    break;
                case CartStatus.Canceled:
                    DateCanceled = DateTime.UtcNow;
                    break;
            }
        }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Для контроля параллелизма
    }
}

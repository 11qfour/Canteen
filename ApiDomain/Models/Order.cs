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
    public class Order
    {
        public Guid OrderId { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        //внешний ключ на клиента (один клиент -> много заказов)
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; }

        public string Address { get; set; } = string.Empty;

        public DateTime? DateConfirmed { get; private set; }
        public DateTime? DateCooking { get; private set; }
        public DateTime? DateReady { get; private set; }
        public DateTime? DateIssued { get; private set; }
        public DateTime? DateCanceled { get; private set; }

        // Метод для обновления временных меток
        public void UpdateStatusTimestamp(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Confirmed:
                    DateConfirmed = DateTime.UtcNow;
                    break;
                case OrderStatus.InCooking:
                    DateCooking = DateTime.UtcNow;
                    break;
                case OrderStatus.Ready:
                    DateReady = DateTime.UtcNow;
                    break;
                case OrderStatus.Issued:
                    DateIssued = DateTime.UtcNow;
                    break;
                case OrderStatus.Canceled:
                    DateCanceled = DateTime.UtcNow;
                    break;
            }
        }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Для контроля параллелизма
    }
}

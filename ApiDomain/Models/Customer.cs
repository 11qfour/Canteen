using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        // Навигационное свойство для связи с корзиной
        public Cart Cart { get; set; } = null!;
        public string NameCustomer { get; set; } = string.Empty;
        public DateTime DateOfBirthday { get; set; }
        public string Email { get; set; } = string.Empty;
        // Навигационное свойство для связи с заказами
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

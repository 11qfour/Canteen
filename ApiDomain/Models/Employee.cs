using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Models
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        //связь с заказом
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public string Position { get; set; } = string.Empty;
    }
}

namespace Api.DTO
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Address { get; set; }
        public string CustomerName { get; set; } // Имя клиента
        public string EmployeeName { get; set; } // Имя сотрудника
    }
}

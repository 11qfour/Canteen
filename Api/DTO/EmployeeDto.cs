using ApiDomain.Models;

namespace Api.DTO
{
    public class EmployeeDto
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
    }
}

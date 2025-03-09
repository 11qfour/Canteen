namespace Api.DTO
{
    public class CustomerCreateDto
    {
        public string NameCustomer { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

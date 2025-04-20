using ApiDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class OrderUpdateDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Confirmed|InCooking|Ready|Issued|NotPaid|Canceled)$", ErrorMessage = "Invalid status.")]
        public OrderStatus Status { get; set; } // Можно менять только статус заказа
    }
}

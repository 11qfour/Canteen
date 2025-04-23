using ApiDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class OrderStatusUpdateDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid status value.")]
        public OrderStatus Status { get; set; }
    }
}

/* 
    Логика перехода между статусами:
    Pending -> Confirmed: Заказ подтвержден.
    Confirmed -> InCooking: Заказ начал готовиться.
    InCooking -> Ready: Заказ готов.
    Ready -> Issued: Заказ выдан клиенту.
    NotPaid -> Issued: Заказ оплачен и выдан.
    Любой статус -> Canceled: Заказ отменен.
 */

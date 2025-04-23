using ApiDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class CartStatusUpdateDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(CartStatus), ErrorMessage = "Invalid status value.")]
        public CartStatus Status { get; set; }
    }

    /*
    Логика перехода между статусами:
    Active -> Ordered: Корзина оформлена как заказ.
    Active -> Canceled: Корзина отменена.
    Ordered: После оформления заказа статус не меняется.
    Canceled: Отмененная корзина не может быть изменена
     */

}

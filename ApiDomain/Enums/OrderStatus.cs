using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Enums
{
    public enum OrderStatus
    {
        Pending,    // В ожидании оформления
        Confirmed,  // Подтвержден, принят в работу
        InCooking,  // Готовится
        Ready,      // Готов
        Issued,     // Выдан клиенту
        NotPaid,    // Не оплачен
        Canceled    // Отменен
    }
}

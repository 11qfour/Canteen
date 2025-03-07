using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Enums
{
    public enum CartStatus
    {
        Active, //в процессе наполнения
        Ordered, //оформленный заказ
        Canceled //заполнение корзины отменено
    }
}

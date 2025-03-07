using ApiDomain.Enums;
using ApiDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Services
{
    public class OrderStatusManager
    {
        // Метод для изменения статуса заказа с логикой проверок
        public void ChangeStatus(Order order, OrderStatus newStatus)
        {
            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                throw new InvalidOperationException($"Неверный переход статуса: {order.Status} → {newStatus}");
            }

            order.Status = newStatus;
            order.UpdateStatusTimestamp(newStatus);
        }
        // Проверка допустимости смены статуса
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Canceled,
                OrderStatus.Confirmed => newStatus == OrderStatus.InCooking || newStatus == OrderStatus.Canceled,
                OrderStatus.InCooking => newStatus == OrderStatus.Ready,
                OrderStatus.Ready => newStatus == OrderStatus.Issued || newStatus == OrderStatus.NotPaid,
                OrderStatus.Issued => false, // Заказ уже выдан, смена статуса невозможна
                OrderStatus.NotPaid => newStatus == OrderStatus.Canceled,
                OrderStatus.Canceled => false, // Отмененный заказ не может изменить статус
                _ => false
            };
        }
    }
}

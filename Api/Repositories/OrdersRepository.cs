using Api.DTO;
using ApiDomain.Enums;
using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiDomain.Repositories
{
    public class OrdersRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public OrdersRepository(ApiListContext dbContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Order>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Order
                .AsNoTracking() //отключает отслеживание сущностей
                .Include(c => c.Customer)
                .Include(c => c.OrderDetails)
                .OrderBy(x => x.Date) //сортируем по дате
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Order
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.OrderDetails)
                .FirstOrDefaultAsync(c => c.OrderId == id, cancellationToken);
        }

        public async Task<List<Order>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Order
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<Order> Add(Guid customerId,decimal totalPrice, string address, List<OrderDetailsDto> orderDetails, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Проверяем существование Customer
                var customerExists = await _dbContext.Customer
                    .AnyAsync(c => c.CustomerId == customerId, cancellationToken);
                if (!customerExists)
                {
                    throw new ArgumentException($"Пользователь с ID {customerId} не найден");
                }
                var orderEntity = new Order
                {
                    CustomerId = customerId,
                    Status = OrderStatus.Pending, //в ожидании оформления
                    TotalPrice = totalPrice,
                    Address = address,
                    OrderDetails = new List<OrderDetails>()
                };
                // Добавляем детали корзины
                foreach (var detail in orderDetails)
                {
                    orderEntity.OrderDetails.Add(new OrderDetails
                    {
                        OrderDetailsId = Guid.NewGuid(),
                        OrderId = orderEntity.OrderId,
                        DishId = detail.DishId,
                        Quantity = detail.Quantity,
                        PriceUnit = detail.PriceUnit
                    });
                }
                var calculatedTotal = orderEntity.OrderDetails.Sum(d => d.Quantity * d.PriceUnit);
                if (Math.Abs(calculatedTotal - totalPrice) > 0.01m)
                {
                    throw new InvalidOperationException("Total price does not match the sum of order details.");
                }
                await _dbContext.AddAsync(orderEntity,cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
                return orderEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при добавлении заказа по адресу {address}");
                throw;
            }
        }
        public async Task Update(Guid orderId,  decimal totalPrice, string address, List<OrderDetailsDto> orderDetails, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var order = await _dbContext.Order
                    .Include(c => c.OrderDetails)
                    .FirstOrDefaultAsync(c => c.OrderId == orderId, cancellationToken);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Заказ с ID {orderId} не найдена.");
                }
                order.TotalPrice=totalPrice;
                order.Address = address;
                // Очищаем старые детали корзины
                if (order.OrderDetails.Any())
                {
                    _dbContext.OrderDetails.RemoveRange(order.OrderDetails);
                    order.OrderDetails.Clear(); // Очищаем коллекцию в памяти
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation($"Удалено деталей заказа: {order.OrderDetails.Count}");

                foreach (var detail in orderDetails)
                {
                    _dbContext.OrderDetails.Add(new OrderDetails
                    {
                        OrderDetailsId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        DishId = detail.DishId,
                        Quantity = detail.Quantity,
                        PriceUnit = detail.PriceUnit
                    });
                }
                _logger.LogInformation($"добавлено деталей заказа: {order.OrderDetails.Count}");
                // Валидация: проверяем совпадение TotalPrice
                var calculatedTotal = order.OrderDetails.Sum(d => d.Quantity * d.PriceUnit);
                if (Math.Abs(calculatedTotal - totalPrice) > 0.01m)
                {
                    throw new InvalidOperationException("Total price does not match the sum of cart details.");
                }

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Ошибка при сохранении заказа");
                throw new InvalidOperationException("Ошибка при создании заказа. Проверьте данные.");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при обновлении заказа {orderId}");
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Order
                .Where(c => c.OrderId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении заказа {id}");
                throw;
            }
        }

        public async Task AddOrderDetails(Guid orderId, OrderDetails orderDetails, CancellationToken cancellationToken) //добавляем детали заказа
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempOrder = await _dbContext.Order
                    .Include(o => o.OrderDetails) // Загружаем связанные записи
                    .FirstOrDefaultAsync(c => c.OrderId == orderId, cancellationToken)
                    ?? throw new Exception($"Ошибка при добавлении деталей заказа {orderDetails.OrderDetailsId} к заказу {orderId}");
                tempOrder.OrderDetails.Add(orderDetails);
                tempOrder.TotalPrice += orderDetails.PriceUnit * orderDetails.Quantity;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Order
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

            if (order == null)
            {
                throw new KeyNotFoundException($"Заказ с ID {orderId} не найден.");
            }

            // Проверяем корректность перехода между статусами
            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                throw new InvalidOperationException($"Недопустимый переход статуса: {order.Status} -> {newStatus}");
            }

            // Обновляем статус и временные метки
            order.Status = newStatus;
            order.UpdateStatusTimestamp(newStatus);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Определяем допустимые переходы между статусами
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Canceled;
                case OrderStatus.Confirmed:
                    return newStatus == OrderStatus.InCooking || newStatus == OrderStatus.Canceled;
                case OrderStatus.InCooking:
                    return newStatus == OrderStatus.Ready || newStatus == OrderStatus.Canceled;
                case OrderStatus.Ready:
                    return newStatus == OrderStatus.Issued || newStatus == OrderStatus.Canceled;
                case OrderStatus.Issued:
                    return false; // После выдачи заказа статус не меняется
                case OrderStatus.NotPaid:
                    return newStatus == OrderStatus.Issued || newStatus == OrderStatus.Canceled;
                case OrderStatus.Canceled:
                    return false; // Отмененный заказ не может быть изменен
                default:
                    return false;
            }
        }
    }
}

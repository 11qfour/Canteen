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
                .Include(c => c.Employee)
                .Include(c => c.OrderDetails)
                .OrderBy(x => x.Date) //сортируем по дате
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Order
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.Employee)
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

        public async Task<Order> Add(Guid customerId,decimal totalPrice, Guid employeeId, string address, CancellationToken cancellationToken)
        {
            try
            {
                var orderEntity = new Order
                {
                    CustomerId = customerId,
                    TotalPrice=totalPrice,
                    EmployeeId=employeeId,
                    Address=address
                };
                await _dbContext.AddAsync(orderEntity,cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                return orderEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при добавлении заказа по адресу {address} покупателю {customerId}");
                throw;
            }
        }
        public async Task Update(Guid orderId, OrderStatus status, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Order
                .Where(c => c.OrderId == orderId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Status, status), cancellationToken);
            }
            catch (Exception e)
            {
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
            var tempOrder = await _dbContext.Order
                .Include(o => o.OrderDetails) // Загружаем связанные записи
                .FirstOrDefaultAsync(c => c.OrderId == orderId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении деталей заказа {orderDetails.OrderDetailsId} к заказу {orderId}");
            tempOrder.OrderDetails.Add(orderDetails);
            tempOrder.TotalPrice += orderDetails.PriceUnit * orderDetails.Quantity;
            await _dbContext.SaveChangesAsync();
        }
    }
}

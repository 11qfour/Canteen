using ApiDomain.Enums;
using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
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
        public OrdersRepository(ApiListContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Order>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Order
                .AsNoTracking() //отключает отслеживание сущностей
                .OrderBy(x => x.Date) //сортируем по дате
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Order
                .AsNoTracking()
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

        public async Task Add(OrderStatus status, Guid customerId,decimal totalPrice, DateTime date, Guid employeeId, string address, CancellationToken cancellationToken)
        {
            try
            {
                var orderEntity = new Order
                {
                    Status = status,
                    CustomerId = customerId,
                    TotalPrice=totalPrice,
                    Date=date,
                    EmployeeId=employeeId,
                    Address=address
                };
                await _dbContext.AddAsync(orderEntity,cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при добавлении заказа по адресу {address} покупателю {customerId}, детали: {e.Message}");
                throw;
            }
        }
        public async Task Update(Guid orderId, OrderStatus status, Guid customerId,decimal totalPrice, DateTime date, Guid employeeId, string address, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Order
                .Where(c => c.OrderId == orderId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Status, status)
                .SetProperty(c => c.CustomerId, customerId)
                .SetProperty(c => c.TotalPrice, totalPrice)
                .SetProperty(c => c.Date, date)
                .SetProperty(c => c.EmployeeId, employeeId)
                .SetProperty(c => c.Address, address), cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при обновлении заказа {orderId}, детали: {e.Message}");
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
                Console.WriteLine($"Ошибка при удалении заказа {id}, детали: {e.Message}");
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

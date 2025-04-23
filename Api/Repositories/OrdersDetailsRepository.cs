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
    public class OrdersDetailsRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public OrdersDetailsRepository(ApiListContext dbContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<OrderDetails>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.OrderDetails
                .Include(c => c.Dish)
                .Include(c => c.Order)
                .OrderBy(c => c.Dish.Price)
                .AsNoTracking() //отключает отслеживание сущностей
                .ToListAsync(cancellationToken);
        }

        public async Task<OrderDetails?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.OrderDetails
                .AsNoTracking()
                .Include(c => c.Dish)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.OrderDetailsId == id, cancellationToken);
        }

        public async Task<List<OrderDetails>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.OrderDetails
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<OrderDetails> Add(Guid orderId, Guid dishId, int quantity, int priceUnit, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Проверяем, существует ли клиент с таким ID
                bool orderExists = await _dbContext.Order.AnyAsync(c => c.OrderId == orderId, cancellationToken);
                if (!orderExists)
                {
                    throw new Exception($"Ошибка: Заказ с ID {orderId} не найден!");
                }
                bool dishExists = await _dbContext.Dish.AnyAsync(c => c.DishId == dishId, cancellationToken);
                if (!dishExists)
                {
                    throw new Exception($"Ошибка: Блюдо с ID {dishId} не найдено!");
                }
                var orderDetailsEntity = new OrderDetails
                {
                    OrderId = orderId,
                    DishId = dishId,
                    Quantity = quantity,
                    PriceUnit = priceUnit
                };
                await _dbContext.AddAsync(orderDetailsEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
                return orderDetailsEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при добавлении деталей заказа у заказа {orderId}");                
                throw;
            }
        }
        public async Task Update(Guid orderDetailsId, Guid orderId, Guid dishId, int quantity, int priceUnit, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var orderDetails = await _dbContext.OrderDetails
                    .Include(c => c.Dish)
                    .Include(c => c.Order)
                    .FirstOrDefaultAsync(c => c.OrderDetailsId == orderDetailsId);
                if (orderDetails == null)
                {
                    throw new KeyNotFoundException($"Детали заказа с ID {orderDetailsId} не найдены.");
                }

                // Проверяем, существует ли клиент с таким ID
                bool orderExists = await _dbContext.Order.AnyAsync(c => c.OrderId == orderId, cancellationToken);
                if (!orderExists)
                {
                    throw new Exception($"Ошибка: Заказ с ID {orderId} не найден!");
                }
                bool dishExists = await _dbContext.Dish.AnyAsync(c => c.DishId == dishId, cancellationToken);
                if (!dishExists)
                {
                    throw new Exception($"Ошибка: Блюдо с ID {dishId} не найдено!");
                }

                orderDetails.OrderId = orderId;
                orderDetails.DishId = dishId;
                orderDetails.Quantity = quantity;
                orderDetails.PriceUnit = priceUnit;
                // Сохраняем изменения
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при обновлении деталей заказа {orderDetailsId}");
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.OrderDetails
                .Where(c => c.OrderDetailsId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении деталей заказа {id}");
                throw;
            }
        }

        public async Task AddFromCartDetails(Guid orderDetailsId, CartDetails cartDetails, CancellationToken cancellationToken) //перенос по одной детали из корзины (нужен цикл)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempOrderDetails = await _dbContext.OrderDetails
                    .FirstOrDefaultAsync(c => c.OrderDetailsId == orderDetailsId, cancellationToken)
                    ?? throw new Exception($"Ошибка при добавлении деталей корзины {cartDetails.CartDetailsId} в детали заказа {orderDetailsId}");
                tempOrderDetails.DishId = cartDetails.DishId;
                tempOrderDetails.Dish = cartDetails.Dish;
                tempOrderDetails.Quantity = cartDetails.Quantity;
                tempOrderDetails.PriceUnit = cartDetails.PriceUnit;
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

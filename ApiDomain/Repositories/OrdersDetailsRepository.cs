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
                .AsNoTracking() //отключает отслеживание сущностей
                .ToListAsync(cancellationToken);
        }

        public async Task<OrderDetails?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.OrderDetails
                .AsNoTracking()
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

        public async Task Add(Guid orderId, Guid dishId, int quantity, int priceUnit, CancellationToken cancellationToken)
        {
            try
            {
                var orderDetailsEntity = new OrderDetails
                {
                    OrderId = orderId,
                    DishId = dishId,
                    Quantity = quantity,
                    PriceUnit = priceUnit
                };
                await _dbContext.AddAsync(orderDetailsEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при добавлении деталей заказа у заказа {orderId}");                
                throw;
            }
        }
        public async Task Update(Guid orderDetailsId, Guid orderId, Guid dishId, int quantity, int priceUnit, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.OrderDetails
                .Where(c => c.OrderDetailsId == orderDetailsId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.OrderId, orderId)
                .SetProperty(c => c.DishId, dishId)
                .SetProperty(c => c.Quantity, quantity)
                .SetProperty(c => c.PriceUnit, priceUnit), cancellationToken);
            }
            catch (Exception e)
            {
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
            var tempOrderDetails = await _dbContext.OrderDetails
                .FirstOrDefaultAsync(c => c.OrderDetailsId == orderDetailsId,cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении деталей корзины {cartDetails.CartDetailsId} в детали заказа {orderDetailsId}");
            tempOrderDetails.DishId = cartDetails.DishId;
            tempOrderDetails.Dish = cartDetails.Dish;
            tempOrderDetails.Quantity = cartDetails.Quantity;
            tempOrderDetails.PriceUnit = cartDetails.PriceUnit;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

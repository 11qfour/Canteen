﻿using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiDomain.Repositories
{
    public class CartDetailsRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public CartDetailsRepository(ApiListContext dbContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<CartDetails>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.CartDetails
                .Include(c=>c.Dish)
                .OrderBy(c=>c.Dish.Price)
                .AsNoTracking() //отключает отслеживание сущностей
                .ToListAsync(cancellationToken);
        }

        public async Task<CartDetails?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.CartDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CartDetailsId == id, cancellationToken);
        }

        public async Task<List<CartDetails>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.CartDetails
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<CartDetails> Add(Guid cartId, Guid dishId, int quantity, decimal priceUnit, CancellationToken cancellationToken)
        {
            try
            {
                var cartDetailsEntity = new CartDetails
                {
                    CartId = cartId,
                    DishId = dishId,
                    Quantity = quantity,
                    PriceUnit = priceUnit
                };
                await _dbContext.AddAsync(cartDetailsEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                return cartDetailsEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при добавлении деталей корзины {cartId}");
                throw;
            }
        }
        public async Task Update(Guid cartDetailsid, Guid cartId, Guid dishId, int quantity, decimal priceUnit, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.CartDetails
                .Where(c => c.CartDetailsId == cartDetailsid)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.CartId, cartId)
                .SetProperty(c => c.DishId, dishId)
                .SetProperty(c => c.Quantity, quantity)
                .SetProperty(c => c.PriceUnit, priceUnit),
                cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при обновлении деталей {cartDetailsid} у корзины {cartId}");
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.CartDetails
                .Where(c => c.CartDetailsId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении деталей корзины {id}");
                throw;
            }
        }

        public async Task AddDish(Guid cartDetailsId, Dish dish, int quantity, CancellationToken cancellationToken) //добавляем работу для работника
        {
            var tempOrder = await _dbContext.CartDetails
                .Include(d=>d.Dish)
                .FirstOrDefaultAsync(c => c.CartDetailsId == cartDetailsId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении блюда {dish.DishId} в детали корзины");
            tempOrder.Dish = dish;
            tempOrder.PriceUnit = dish.Price; //устанавливаем цену
            tempOrder.Quantity = quantity;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

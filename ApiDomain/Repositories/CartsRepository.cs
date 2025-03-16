using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Repositories
{
    public class CartsRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public CartsRepository(ApiListContext apiDBContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = apiDBContext;
            _logger = logger;
        }

        public async Task<List<Cart>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Cart
                .AsNoTracking() //отключает отслеживание сущностей
                .Include(c => c.Customer)
                .Include(c => c.CartDetails)
                .OrderBy(c=>c.Status) //по наполнению корзины
                .ToListAsync(cancellationToken);
        }

        public async Task<Cart?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Cart
                .AsNoTracking()
                .Include(c => c.Customer)
                .Include(c => c.CartDetails)// Загружаем детали корзины
                .FirstOrDefaultAsync(c => c.CartId ==id, cancellationToken);
        }

        public async Task<List<Cart>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Cart
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<Cart> Add(Guid customerId, decimal totalPrice, CancellationToken cancellationToken)
        {
            try
            {
                var cartEntity = new Cart
                {
                    CustomerId = customerId,
                    Status = Enums.CartStatus.Active,
                    TotalPrice=totalPrice
                };
                await _dbContext.AddAsync(cartEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                return cartEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при добавлении корзины покупателя {customerId}");
                throw;
            }
        }

        /*public async Task Update2 (int id, int customerId)
        {
            var cartEntity = await _dbContext.Cart.FirstOrDefaultAsync(c => c.CartID == id)
                ?? throw new Exception();

            cartEntity.CartID = id;
            cartEntity.CustomerId = customerId;

            await _dbContext.SaveChangesAsync(); 
        }*/

        public async Task Update(Guid id, Guid customerId, decimal totalPrice,CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем, существует ли клиент с таким ID
                bool customerExists = await _dbContext.Customer.AnyAsync(c => c.CustomerId == customerId, cancellationToken);
                if (!customerExists)
                {
                    throw new Exception($"Ошибка: Клиент с ID {customerId} не найден!");
                }
                await _dbContext.Cart
                .Where(c => c.CartId == id)
                .Include(c=>c.Customer)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.CustomerId, customerId)
                .SetProperty(c=>c.TotalPrice, totalPrice)
                , cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при обновлении корзины {id}");
                throw;
            }
}

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Cart
                .Where(c => c.CartId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении корзины {id}");
                throw;
            }
        }

        public async Task AddCartDetails(Guid cartId, CartDetails cartDetails, CancellationToken cancellationToken) //добавляем детали корзины в корзину
        {
            var tempCart = await _dbContext.Cart.
                Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CartId == cartId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении деталей корзины {cartDetails.CartDetailsId} в детали корзины");
            tempCart.CartDetails.Add(cartDetails);
            tempCart.TotalPrice += cartDetails.PriceUnit * cartDetails.Quantity;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

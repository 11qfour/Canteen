using ApiDomain.Models;
using ApiDomain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.DTO;


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

        public async Task<Cart> Add(Guid customerId, decimal totalPrice, List<CartDetailsDto> cartDetails,CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Проверка, существует ли клиент
                var customer = await _dbContext.Customer
                    .Include(c => c.Cart)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

                if (customer == null)
                {
                    throw new Exception($"Ошибка: Клиент с ID {customerId} не найден!");
                }
                // Проверка, есть ли уже корзина
                if (customer.Cart != null)
                {
                    throw new InvalidOperationException("У клиента уже есть корзина. Допускается только одна активная корзина.");
                }
                var cartEntity = new Cart
                {
                    CustomerId = customerId,
                    Status = CartStatus.Active,
                    TotalPrice=totalPrice,
                    CartDetails = new List<CartDetails>()
                };
                // Добавляем детали корзины
                foreach (var detail in cartDetails)
                {
                    cartEntity.CartDetails.Add(new CartDetails
                    {
                        CartDetailsId = Guid.NewGuid(),
                        CartId = cartEntity.CartId,
                        DishId = detail.DishId,
                        Quantity = detail.Quantity,
                        PriceUnit = detail.PriceUnit
                    });
                }
                var calculatedTotal = cartEntity.CartDetails.Sum(d => d.Quantity * d.PriceUnit);
                if (Math.Abs(calculatedTotal - totalPrice) > 0.01m)
                {
                    throw new InvalidOperationException("Total price does not match the sum of cart details.");
                }
                // Сохраняем изменения
                await _dbContext.Cart.AddAsync(cartEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
                return cartEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при добавлении корзины покупателя {customerId}");
                throw;
            }
        }

        public async Task Update(Guid id, Guid customerId, decimal totalPrice, List<CartDetailsDto> cartDetails, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Загружаем корзину с трекингом (без AsNoTracking!)
                var cart = await _dbContext.Cart
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.CartId == id, cancellationToken);

                if (cart == null)
                {
                    throw new KeyNotFoundException($"Корзина с ID {id} не найдена.");
                }

                // Проверка клиента
                bool customerExists = await _dbContext.Customer.AnyAsync(c => c.CustomerId == customerId, cancellationToken);
                if (!customerExists)
                {
                    throw new Exception($"Ошибка: Клиент с ID {customerId} не найден!");
                }

                // Обновление корзины
                cart.CustomerId = customerId;
                cart.TotalPrice = totalPrice;

                // Удаление старых деталей
                if (cart.CartDetails.Any())
                {
                    _dbContext.CartDetails.RemoveRange(cart.CartDetails);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation($"Удалено деталей корзины: {cart.CartDetails.Count}");

                // Добавление новых
                foreach (var detail in cartDetails)
                {
                    _dbContext.CartDetails.Add(new CartDetails
                    {
                        CartDetailsId = Guid.NewGuid(),
                        CartId = cart.CartId,
                        DishId = detail.DishId,
                        Quantity = detail.Quantity,
                        PriceUnit = detail.PriceUnit
                    });
                }

                _logger.LogInformation($"Добавлено новых деталей: {cartDetails.Count}");

                // Проверка суммы
                var calculatedTotal = cartDetails.Sum(d => d.Quantity * d.PriceUnit);
                _logger.LogInformation($"Цена итоговая: {calculatedTotal}");

                if (Math.Abs(calculatedTotal - totalPrice) > 0.01m)
                {
                    throw new InvalidOperationException("Total price does not match the sum of cart details.");
                }

                // Сохраняем
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, "Конфликт версий при обновлении корзины");
                throw new InvalidOperationException("Данные были изменены другим пользователем.");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
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
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempCart = await _dbContext.Cart
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.CartId == cartId, cancellationToken)
                    ?? throw new Exception($"Корзина с ID {cartId} не найдена.");

                tempCart.CartDetails.Add(cartDetails);
                tempCart.TotalPrice += cartDetails.PriceUnit * cartDetails.Quantity;

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateStatus(Guid cartId, CartStatus newStatus, CancellationToken cancellationToken)
        {
            var cart = await _dbContext.Cart
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CartId == cartId, cancellationToken);

            if (cart == null)
            {
                throw new KeyNotFoundException($"Корзина с ID {cartId} не найдена.");
            }

            // Проверяем корректность перехода между статусами
            if (!IsValidStatusTransition(cart.Status, newStatus))
            {
                throw new InvalidOperationException($"Недопустимый переход статуса: {cart.Status} -> {newStatus}");
            }

            // Обновляем статус и временные метки
            cart.Status = newStatus;
            cart.UpdateStatusTimestamp(newStatus);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private bool IsValidStatusTransition(CartStatus currentStatus, CartStatus newStatus)
        {
            // Определяем допустимые переходы между статусами
            switch (currentStatus)
            {
                case CartStatus.Active:
                    return newStatus == CartStatus.Ordered || newStatus == CartStatus.Canceled;
                case CartStatus.Ordered:
                    return false; // После оформления заказа статус не меняется
                case CartStatus.Canceled:
                    return false; // Отмененная корзина не может быть изменена
                default:
                    return false;
            }
        }
    }
}

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
    public class CustomersRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public CustomersRepository(ApiListContext apiDBContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = apiDBContext;
            _logger = logger;
        }

        public async Task<List<Customer>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Customer
                .Include(c => c.Orders)
                .AsNoTracking() //отключает отслеживание сущностей
                .ToListAsync(cancellationToken);
        }

        public async Task <Customer?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Customer
                .AsNoTracking()
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id,cancellationToken);
        }

        public async Task<List<Customer>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Customer
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<Customer> Add(string nameCustomer, DateTime dataBirth, string email, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var customerEntity = new Customer
                {
                    NameCustomer = nameCustomer,
                    DateOfBirthday = dataBirth,
                    Email=email
                };
                await _dbContext.AddAsync(customerEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                await transaction.CommitAsync(cancellationToken);
                return customerEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при добавлении покупателя {nameCustomer}");
                throw;
            }
        }

        public async Task Update(Guid customerId, string nameCustomer, string email, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var customer = await _dbContext.Customer
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Корзина с ID {customerId} не найдена.");
                }

                customer.NameCustomer = nameCustomer;
                customer.Email = email;

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при обновлении покупателя {customerId}");
                throw;
            }
        }

        public async Task Delete(Guid id,CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Customer
                .Where(c => c.CustomerId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении покупателя {id}");               
                throw;
            }
        }

        public async Task AddCart(Guid customerId, Cart cart, CancellationToken cancellationToken) //присваиваем корзину юзеру
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempCustomer = await _dbContext.Customer
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken)
                    ?? throw new Exception($"Ошибка при добавлении корзины пользователю {cart.CartId} к продавцу {customerId}");
                tempCustomer.Cart = cart;
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task AddOrder(Guid customerId, Order order, CancellationToken cancellationToken) //добавляем заказ в список заказов пользователя
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempEmployee = await _dbContext.Customer
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken)
                    ?? throw new Exception($"Ошибка при добавлении заказа {order.OrderId} к продавцу {customerId}");
                tempEmployee.Orders.Add(order);
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

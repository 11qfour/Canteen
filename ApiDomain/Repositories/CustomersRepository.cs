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
    public class CustomersRepository
    {
        private readonly ApiListContext _dbContext;
        public CustomersRepository(ApiListContext apiDBContext)
        {
            _dbContext = apiDBContext;
        }

        public async Task<List<Customer>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Customer
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

        public async Task Add(string nameCustomer, DateTime dataBirth, string email, CancellationToken cancellationToken)
        {
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
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при добавлении покупателя {nameCustomer}, детали: {e.Message}");
                throw;
            }
        }

        public async Task Update(Guid customerId, string nameCustomer, DateTime dataBirth, string email, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Customer
                .Where(c => c.CustomerId == customerId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.NameCustomer, nameCustomer)
                .SetProperty(c => c.DateOfBirthday, dataBirth)
                .SetProperty(c => c.Email, email), cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при обновлении покупателя {customerId}, детали: {e.Message}");
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
                Console.WriteLine($"Ошибка при удалении покупателя {id}, детали: {e.Message}");
                throw;
            }
        }

        public async Task AddCart(Guid customerId, Cart cart, CancellationToken cancellationToken) //присваиваем корзину юзеру
        {
            var tempCustomer = await _dbContext.Customer
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении корзины пользователю {cart.CartId} к продавцу {customerId}");
            tempCustomer.Cart = cart;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddOrder(Guid customerId, Order order, CancellationToken cancellationToken) //добавляем заказ в список заказов пользователя
        {
            var tempEmployee = await _dbContext.Customer
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении заказа {order.OrderId} к продавцу {customerId}");
            tempEmployee.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

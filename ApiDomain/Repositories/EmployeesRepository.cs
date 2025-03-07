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
    public class EmployeesRepository
    {
        private readonly ApiListContext _dbContext;
        public EmployeesRepository(ApiListContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Employee>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Employee
                .AsNoTracking() //отключает отслеживание сущностей
                .OrderBy(x=>x.Position) //сортируем по позиции
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Employee
                .AsNoTracking()
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.EmployeeId == id, cancellationToken);
        }

        public async Task<List<Employee>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Employee
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task Add(string fullName, string phoneNumber, string position, CancellationToken cancellationToken)
        {
            try
            {
                var employeeEntity = new Employee
                {
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Position=position
                };
                await _dbContext.AddAsync(employeeEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при добавлении сотрудника {fullName}, детали: {e.Message}");
                throw;
            }
        }
        public async Task Update(Guid employeeId, string fullName, string phoneNumber, string position, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Employee
                .Where(c => c.EmployeeId == employeeId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.FullName,fullName)
                .SetProperty(c => c.PhoneNumber, phoneNumber)
                .SetProperty(c => c.Position, position), cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при обновлении сотрудника {employeeId}, детали: {e.Message}");
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Employee
                .Where(c => c.EmployeeId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при удалении сотрудника {id}, детали: {e.Message}");
                throw;
            }
        }

        public async Task AddOrder(Guid employeeId, Order order, CancellationToken cancellationToken) //добавляем работу для работника
        {
            var tempEmployee = await _dbContext.Employee
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId, cancellationToken)
                ?? throw new Exception($"Ошибка при добавлении заказа {order.OrderId} к сотруднику {employeeId}");
            tempEmployee.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

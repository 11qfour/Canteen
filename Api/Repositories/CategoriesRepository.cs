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
    public class CategoriesRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public CategoriesRepository(ApiListContext dbContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Category>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Category
                .AsNoTracking() //отключает отслеживание сущностей
                .Include(c => c.Dishes)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Category
                .AsNoTracking()
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
        }

        public async Task<List<Category>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Category
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<Category> Add(string nameCategory, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var categoryEntity = new Category
                {
                    NameCategory = nameCategory
                };
                await _dbContext.AddAsync(categoryEntity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
                return categoryEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при добавлении категории {nameCategory}");
                throw;
            }
        }
        public async Task Update(Guid categoryId, string nameCategory,CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var category = await _dbContext.Category
                    .Include(c=>c.Dishes)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Категория с ID {categoryId} не найдена.");
                }
                category.NameCategory = nameCategory;
                // Сохраняем изменения
                await _dbContext.SaveChangesAsync(cancellationToken);
                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при обновлении категории {categoryId}");
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Category
                .Where(c => c.CategoryId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении категории {id}");
                throw;
            }
        }

        public async Task AddDish(Guid categoryId, Dish dish, CancellationToken cancellationToken) //добавляем блюдо в список блюд
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var tempCategory = await _dbContext.Category
                    .Include(c => c.Dishes) // Загружаем список блюд
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId, cancellationToken)
                    ?? throw new Exception($"Ошибка при добавлении блюда {dish.DishId} в категорию {categoryId}");
                tempCategory.Dishes.Add(dish);
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

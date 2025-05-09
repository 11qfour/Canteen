﻿using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Repositories
{
    public class DishesRepository
    {
        private readonly ApiListContext _dbContext;
        private readonly ILogger<OrdersDetailsRepository> _logger;
        public DishesRepository(ApiListContext apiDBContext, ILogger<OrdersDetailsRepository> logger)
        {
            _dbContext = apiDBContext;
            _logger = logger;
        }

        public async Task<List<Dish>> Get(CancellationToken cancellationToken)
        {
            return await _dbContext.Dish
                .AsNoTracking() //отключает отслеживание сущностей
                .Include(d => d.Category)
                .OrderBy(c => c.CategoryId) //по айди категории
                .ToListAsync(cancellationToken);
        }

        public async Task<Dish?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Dish
                .AsNoTracking()
                .Include(d => d.Category) //включаем подгрузку категорий
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.DishId == id, cancellationToken);
        }

        public async Task<List<Dish>> GetByPage(int page, int pageSize, CancellationToken cancellationToken)//пагинация
        {
            return await _dbContext.Dish
                .AsNoTracking()
                .Skip((page - 1) * pageSize) //пропускаем ненужные страницы
                .Take(pageSize) //беремн нужное количесво элементов
                .ToListAsync(cancellationToken);
        }

        public async Task<Dish> Add(string dishName,string description, decimal price, Guid categoryId, int cookingTime, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Проверяем, существует ли клиент с таким ID
                bool categoryExists = await _dbContext.Category.AnyAsync(c => c.CategoryId == categoryId, cancellationToken);
                if (!categoryExists)
                {
                    throw new Exception($"Ошибка: Категория с ID {categoryId} не найден!");
                }
                var dishEntity = new Dish
                {
                    DishName = dishName,
                    Description = description,
                    Price = price,
                    CategoryId = categoryId,
                    CookingTime = cookingTime
                };
                await _dbContext.AddAsync(dishEntity,cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);//сохраним данные
                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
                return dishEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при добавлении блюда {dishName}");
                throw;
            }
        }

        public async Task Update(Guid dishId, string dishName, string description, decimal price, int cookingTime, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var dish = await _dbContext.Dish
                    .Include(d => d.Category) //включаем подгрузку категорий
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.DishId == dishId);

                if(dish==null)
                    throw new KeyNotFoundException($"Блюдо с ID {dishId} не найдено.");



                dish.DishName = dishName;
                dish.Price = price;
                dish.CookingTime = cookingTime;
                dish.Description = description;

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Подтверждаем транзакцию
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Ошибка при обновлении блюда {dishId}");               
                throw;
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Dish
                .Where(c => c.DishId == id)
                .ExecuteDeleteAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при удалении блюда {id}");
                throw;
            }
        }
    }
}

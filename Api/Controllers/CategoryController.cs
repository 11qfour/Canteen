using Api.DTO;
using ApiDomain.Models;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoriesRepository _categoriesRepository;
        public CategoryController(CategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _categoriesRepository.Get(cancellationToken);
            var categoryDtos = categories.Select(category => new CategoryDto
            {
                CategoryId = category.CategoryId,
                NameCategory = category.NameCategory,
                Dishes = category.Dishes.Select(d => new DishDto
                {
                    DishId = d.DishId,
                    DishName = d.DishName, // Получаем название блюда
                    Description = d.Description,
                    Price = d.Price,
                    CategoryName = category.NameCategory
                }).ToList()
            }).ToList();
            return Ok(categoryDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categoriesRepository.GetById(id, cancellationToken);
            if (category == null)
                return NotFound($"Категория {id} не найдена!");

            var categoryDto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                NameCategory = category.NameCategory
            };
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto categoryDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) //проверяем заполненность данных в модели Дто
                return BadRequest(ModelState);

            try
            {
                var createdCategory = await _categoriesRepository.Add(categoryDto.NameCategory, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto categoryDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _categoriesRepository.Update(id, categoryDto.NameCategory, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _categoriesRepository.Delete(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

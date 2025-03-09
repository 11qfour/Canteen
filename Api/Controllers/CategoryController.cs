using Api.DTO;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/category")]
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
                NameCategory = category.NameCategory
            }).ToList();
            return Ok(categoryDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categoriesRepository.GetById(id, cancellationToken);
            if (category == null)
                return NotFound($"Блюдо {id} не найдено!");

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
            if (categoryDto == null)
                return BadRequest("Некорректные данные");

            var createdCategory = await _categoriesRepository.Add(categoryDto.NameCategory, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId } ,categoryDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto categoryDto, CancellationToken cancellationToken)
        {
            if (categoryDto == null)
                return BadRequest("Некорректные данные");

            await _categoriesRepository.Update(id, categoryDto.NameCategory, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _categoriesRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

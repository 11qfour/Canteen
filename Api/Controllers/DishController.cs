using Api.DTO;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly DishesRepository _dishesRepository;
        public DishController(DishesRepository dishesRepository)
        {
            _dishesRepository = dishesRepository;
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var dishes = await _dishesRepository.Get(cancellationToken);
            var dishDtos = dishes.Select(dish => new DishDto
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                Description = dish.Description,
                Price = dish.Price,
                CategoryName = dish.Category?.NameCategory ?? "No Category" // проверка на существование
            }).ToList();
            return Ok(dishDtos);
        }
        [Authorize(Roles = "User")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dish = await _dishesRepository.GetById(id, cancellationToken);
            if (dish == null)
                return NotFound($"Блюдо {id} не найдено!");

            var dishDto = new DishDto
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                Description = dish.Description,
                Price = dish.Price,
                CategoryName = dish.Category?.NameCategory ?? "No Category" // проверка на существование
            };
            return Ok(dishDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DishCreateDto dishDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) //проверяем заполненность данных в модели Дто
                return BadRequest(ModelState);

            try
            {
                var newDish = await _dishesRepository.Add(dishDto.DishName, dishDto.Description, dishDto.Price, dishDto.CategoryId, dishDto.CookingTime, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = newDish.DishId}, dishDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DishUpdateDto dishDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _dishesRepository.Update(id, dishDto.DishName, dishDto.Description, dishDto.Price, dishDto.CookingTime, cancellationToken);
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _dishesRepository.Delete(id, cancellationToken);
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

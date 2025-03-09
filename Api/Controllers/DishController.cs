using Api.DTO;
using ApiDomain.Repositories;
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

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var dishes = await _dishesRepository.Get(cancellationToken);
            var dishDtos = dishes.Select(dish => new DishDto
            {
                DishId=dish.DishId,
                DishName = dish.DishName,
                Description = dish.Description,
                Price = dish.Price,
                CategoryName = dish.Category.NameCategory
            }).ToList();
            return Ok(dishDtos);
        }

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
                CategoryName = dish.Category.NameCategory
            };
            return Ok(dishDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DishCreateDto dishDto, CancellationToken cancellationToken)
        {
            if (dishDto == null)
                return BadRequest("Некорректные данные");

            var newDish = await _dishesRepository.Add(dishDto.DishName, dishDto.Description, dishDto.Price, dishDto.CategoryId, dishDto.CookingTime, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = newDish.DishId}, dishDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DishUpdateDto dishDto, CancellationToken cancellationToken)
        {
            if (dishDto == null)
                return BadRequest("Некорректные данные");

            await _dishesRepository.Update(id, dishDto.DishName, dishDto.Description, dishDto.Price, dishDto.CookingTime, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _dishesRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

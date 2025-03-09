using Api.DTO;
using ApiDomain;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Api.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartsRepository _cartsRepository;
        public CartController(CartsRepository cartsRepository)
        {
            _cartsRepository = cartsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)  // Получение всех корзин
        {
            var carts = await _cartsRepository.Get(cancellationToken);
            var cartDtos = carts.Select(cart => new CartDto //создали объекты для вывода
            {
                CartId = cart.CartId,
                CustomerName = cart.Customer.NameCustomer,
                TotalPrice = cart.TotalPrice,
                Status = cart.Status.ToString(),
                CartDetails = cart.CartDetails.Select(d=> new CartDetailsDto
                {
                    DishId = d.DishId,
                    DishName = d.Dish.DishName, // Получаем название блюда
                    Quantity = d.Quantity,
                    PriceUnit = d.PriceUnit
                }).ToList()
            }).ToList();
            return Ok(cartDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var cart = await _cartsRepository.GetById(id, cancellationToken);
            if (cart == null)
                return NotFound($"Корзина {id} не найдена!");
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                CustomerName = cart.Customer.NameCustomer,
                TotalPrice = cart.TotalPrice,
                Status = cart.Status.ToString(),
                CartDetails = cart.CartDetails.Select(d => new CartDetailsDto
                {
                    DishId = d.DishId,
                    DishName = d.Dish.DishName,
                    Quantity = d.Quantity,
                    PriceUnit = d.PriceUnit
                }).ToList()
            };
            return Ok(cartDto);
        }

        [HttpPost]// Добавление корзины
        public async Task<IActionResult> Add([FromBody] CartCreateDto cartDto, CancellationToken cancellationToken)
        {
            if (cartDto == null)
                return BadRequest("Некорректные данные");

            var newCart = await _cartsRepository.Add(cartDto.CustomerId, cartDto.TotalPrice, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = newCart.CartId }, cartDto);
        }

        // Обновление корзины
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartUpdateDto cartDto, CancellationToken cancellationToken)
        {
            if (cartDto == null)
                return BadRequest("Некорректные данные");

            await _cartsRepository.Update(id, cartDto.CustomerId, cartDto.TotalPrice, cancellationToken);
            return NoContent();
        }

        // Удаление корзины
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _cartsRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

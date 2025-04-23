using Api.DTO;
using ApiDomain;
using ApiDomain.Enums;
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
                    Quantity = d.Quantity,
                    PriceUnit = d.PriceUnit
                }).ToList()
            };
            return Ok(cartDto);
        }

        [HttpPost]// Добавление корзины
        public async Task<IActionResult> Add([FromBody] CartCreateDto cartDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) //проверяем заполненность данных в модели Дто
                return BadRequest(ModelState);

            try
            {
                var newCart = await _cartsRepository.Add(cartDto.CustomerId, cartDto.TotalPrice, cartDto.CartDetails, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = newCart.CartId }, cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Обновление корзины
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartUpdateDto cartDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cartsRepository.Update(id, cartDto.CustomerId, cartDto.TotalPrice, cartDto.CartDetails, cancellationToken);
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

        // Удаление корзины
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _cartsRepository.Delete(id, cancellationToken);
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

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CartStatusUpdateDto statusDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Enum.IsDefined(typeof(CartStatus), statusDto.Status))
            {
                return BadRequest("Некорректные данные статуса."); // Если значение выходит за границы enum
            }

            try
            {
                await _cartsRepository.UpdateStatus(id, statusDto.Status, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Возвращаем ошибку при недопустимом переходе статуса
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

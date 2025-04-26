using Api.DTO;
using ApiDomain.Enums;
using ApiDomain.Models;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrdersRepository _ordersRepository;
        public OrderController(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)  
        {
            var orders = await _ordersRepository.Get(cancellationToken);
            var orderDtos = orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                TotalPrice=order.TotalPrice,
                Address = order.Address,
                CustomerName = order.Customer.NameCustomer,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailsDto
                {
                    DishId = d.DishId,
                    Quantity = d.Quantity,
                    PriceUnit = d.PriceUnit
                }).ToList()
            }).ToList();
            return Ok(orderDtos);
        }
        [Authorize(Roles = "User")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var order = await _ordersRepository.GetById(id, cancellationToken);
            if (order == null)
                return NotFound($"Заказ {id} не найден!");

            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                Address = order.Address,
                CustomerName = order.Customer.NameCustomer,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailsDto
                {
                    DishId = d.DishId,
                    Quantity = d.Quantity,
                    PriceUnit = d.PriceUnit
                }).ToList()
            };
            return Ok(orderDto);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderCreateDto orderDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) //проверяем заполненность данных в модели Дто
                return BadRequest(ModelState);

            try
            {
                var newOrder = await _ordersRepository.Add(orderDto.CustomerId, orderDto.TotalPrice, orderDto.Address, orderDto.OrderDetails,cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = newOrder.OrderId }, orderDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "User")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderUpdateDto orderDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
/*            if (!Enum.IsDefined(typeof(OrderStatus), orderDto.Status))
            {
                return BadRequest("Некорректные данные статуса"); //если выходит за границы enum
            }*/
            try
            {
                await _ordersRepository.Update(id, orderDto.TotalPrice, orderDto.Address, orderDto.OrderDetails ,cancellationToken);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Возвращаем ошибку при недопустимом переходе статуса
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
                await _ordersRepository.Delete(id, cancellationToken);
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
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatusUpdateDto statusDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Enum.IsDefined(typeof(OrderStatus), statusDto.Status))
            {
                return BadRequest("Некорректные данные статуса."); // Если значение выходит за границы enum
            }

            try
            {
                await _ordersRepository.UpdateStatus(id, statusDto.Status, cancellationToken);
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

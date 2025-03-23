using Api.DTO;
using ApiDomain.Enums;
using ApiDomain.Models;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrdersRepository _ordersRepository;
        public OrderController(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

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
                EmployeeName=order.Employee.FullName
            }).ToList();
            return Ok(orderDtos);
        }

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
                EmployeeName = order.Employee.FullName
            };
            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderCreateDto orderDto, CancellationToken cancellationToken)
        {
            if (orderDto == null)
                return BadRequest("Некорректные данные");

            var newOrder = await _ordersRepository.Add(orderDto.CustomerId, orderDto.TotalPrice, orderDto.EmployeeId, orderDto.Address, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = newOrder.OrderId }, orderDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderUpdateDto orderDto, CancellationToken cancellationToken)
        {
            if (orderDto == null)
                return BadRequest("Некорректные данные");
            if (!Enum.IsDefined(typeof(OrderStatus), orderDto.Status))
            {
                return BadRequest("Некорректные данные"); //если выходит за границы enum
            }
            await _ordersRepository.Update(id,orderDto.Status, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _ordersRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

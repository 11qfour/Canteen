using Api.DTO;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomersRepository _customersRepository;
        public CustomerController(CustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)  
        {
            var customers = await _customersRepository.Get(cancellationToken);
            var customerDtos = customers.Select(customer => new CustomerDto
            {
                CustomerId=customer.CustomerId,
                NameCustomer=customer.NameCustomer,
                Email=customer.Email,
                orders=customer.Orders.Select(d=>new OrderDto
                {
                    OrderId=d.OrderId,
                    Status=d.Status.ToString(),
                    TotalPrice=d.TotalPrice,
                    Address=d.Address,
                    CustomerName=d.Customer.NameCustomer
                    /*OrderDetails=d.OrderDetails.Select(x => new OrderDetailsDto
                    {
                        DishId = x.DishId,
                        Quantity = x.Quantity,
                        PriceUnit = x.PriceUnit
                    }).ToList()*/
                }).ToList()
            }).ToList();
            return Ok(customerDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var customer = await _customersRepository.GetById(id, cancellationToken);
            if (customer == null)
                return NotFound($"Покупатель {id} не найден!");

            var customerDto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                NameCustomer = customer.NameCustomer,
                Email = customer.Email,
                orders = customer.Orders.Select(d => new OrderDto
                {
                    OrderId = d.OrderId,
                    Status = d.Status.ToString(),
                    TotalPrice = d.TotalPrice,
                    Address = d.Address,
                    CustomerName = d.Customer.NameCustomer
                    /*                    OrderDetails = d.OrderDetails.Select(x => new OrderDetailsDto
                                        {
                                            DishId = x.DishId,
                                            Quantity = x.Quantity,
                                            PriceUnit = x.PriceUnit
                                        }).ToList()*/
                }).ToList()
            };
            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerCreateDto customerDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var newCustomer = await _customersRepository.Add(customerDto.NameCustomer, customerDto.DateOfBirth, customerDto.Email, cancellationToken);
                return CreatedAtAction(nameof(GetById),new { id = newCustomer.CustomerId }, customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateDto customerDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _customersRepository.Update(id, customerDto.NameCustomer, customerDto.Email, cancellationToken);
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
                await _customersRepository.Delete(id, cancellationToken);
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

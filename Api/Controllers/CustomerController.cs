using Api.DTO;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
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
                Email=customer.Email
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
                Email = customer.Email
            };
            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerCreateDto customerDto, CancellationToken cancellationToken)
        {
            if (customerDto == null)
                return BadRequest("Некорректные данные");

            var newCustomer = await _customersRepository.Add(customerDto.NameCustomer, customerDto.DateOfBirth, customerDto.Email, cancellationToken);
            return CreatedAtAction(nameof(GetById),new { id = newCustomer.CustomerId }, customerDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateDto customerDto, CancellationToken cancellationToken)
        {
            if (customerDto == null)
                return BadRequest("Некорректные данные");

            await _customersRepository.Update(id, customerDto.NameCustomer, customerDto.Email, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _customersRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

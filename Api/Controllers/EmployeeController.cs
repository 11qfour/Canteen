using Api.DTO;
using ApiDomain.Models;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeesRepository _employeesRepository;
        public EmployeeController(EmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var employees = await _employeesRepository.Get(cancellationToken);
            var employeeDtos = employees.Select(employee => new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                PhoneNumber=employee.PhoneNumber,
                Position=employee.Position
            }).ToList();
            return Ok(employeeDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository.GetById(id, cancellationToken);
            if (employee == null)
                return NotFound($"Блюдо {id} не найдено!");

            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                PhoneNumber = employee.PhoneNumber,
                Position = employee.Position
            };
            return Ok(employeeDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EmployeeCreateDto employeeDto, CancellationToken cancellationToken)
        {
            if (employeeDto == null)
                return BadRequest("Некорректные данные");

            var createdCategory = await _employeesRepository.Add(employeeDto.FullName,employeeDto.PhoneNumber,employeeDto.Position, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdCategory.EmployeeId }, employeeDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeUpdateDto employeeDto, CancellationToken cancellationToken)
        {
            if (employeeDto == null)
                return BadRequest("Некорректные данные");

            await _employeesRepository.Update(id, employeeDto.FullName, employeeDto.PhoneNumber, employeeDto.Position, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _employeesRepository.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}

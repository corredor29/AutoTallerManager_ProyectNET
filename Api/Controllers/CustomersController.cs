using Api.Security;
using Application.Common.Pagination;
using Application.Contracts.Services;
using Application.Filters;
using Application.Requests.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo Customers.
    public sealed class CustomersController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaginationParams pagination,
            [FromQuery] CustomerFilter filter)
        {
            var result = await _customerService.GetAllPagedAsync(pagination, filter);
            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            return Ok(result);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var customer = await _customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

    // Registra un cliente y su vehiculo en una sola operacion.
        [HttpPost("register-with-vehicle")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> RegisterWithVehicle([FromBody] RegisterCustomerWithVehicleRequest request)
        {
            var result = await _customerService.RegisterWithVehicleAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Customer.Id }, result);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
        {
            var updated = await _customerService.UpdateAsync(id, request);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _customerService.DeleteAsync(id);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

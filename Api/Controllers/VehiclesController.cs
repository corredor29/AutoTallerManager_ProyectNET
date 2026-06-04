using Api.Security;
using Application.Common.Pagination;
using Application.Contracts.Services;
using Application.Filters;
using Application.Requests.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo Vehicles.
    public sealed class VehiclesController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaginationParams pagination,
            [FromQuery] VehicleFilter filter)
        {
            var result = await _vehicleService.GetAllPagedAsync(pagination, filter);
            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            return Ok(result);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle is null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            var vehicle = await _vehicleService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleRequest request)
        {
            var updated = await _vehicleService.UpdateAsync(id, request);
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
            var removed = await _vehicleService.DeleteAsync(id);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

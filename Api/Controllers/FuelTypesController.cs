using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.FuelTypes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo FuelTypes.
    public sealed class FuelTypesController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IFuelTypeService _fuelTypeService;

        public FuelTypesController(IFuelTypeService fuelTypeService)
        {
            _fuelTypeService = fuelTypeService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fuelTypes = await _fuelTypeService.GetAllAsync();
            return Ok(fuelTypes);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fuelType = await _fuelTypeService.GetByIdAsync(id);
            if (fuelType is null)
                return NotFound();

            return Ok(fuelType);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateFuelTypeRequest request)
        {
            var fuelType = await _fuelTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = fuelType.Id }, fuelType);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFuelTypeRequest request)
        {
            var updated = await _fuelTypeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _fuelTypeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

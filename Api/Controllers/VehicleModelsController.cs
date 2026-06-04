using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.VehicleModels;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo VehicleModels.
    public sealed class VehicleModelsController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IVehicleModelService _vehicleModelService;

        public VehicleModelsController(IVehicleModelService vehicleModelService)
        {
            _vehicleModelService = vehicleModelService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var models = await _vehicleModelService.GetAllAsync();
            return Ok(models);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _vehicleModelService.GetByIdAsync(id);
            if (model is null)
                return NotFound();

            return Ok(model);
        }

    // Obtiene los modelos asociados a una marca concreta.
        [HttpGet("brand/{brandId:int}")]
        public async Task<IActionResult> GetByBrandId(int brandId)
        {
            var models = await _vehicleModelService.GetByBrandIdAsync(brandId);
            return Ok(models);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleModelRequest request)
        {
            var model = await _vehicleModelService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleModelRequest request)
        {
            var updated = await _vehicleModelService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _vehicleModelService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

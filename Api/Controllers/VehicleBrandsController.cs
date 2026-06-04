using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.VehicleBrands;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo VehicleBrands.
    public sealed class VehicleBrandsController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IVehicleBrandService _vehicleBrandService;

        public VehicleBrandsController(IVehicleBrandService vehicleBrandService)
        {
            _vehicleBrandService = vehicleBrandService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _vehicleBrandService.GetAllAsync();
            return Ok(brands);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _vehicleBrandService.GetByIdAsync(id);
            if (brand is null)
                return NotFound();

            return Ok(brand);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleBrandRequest request)
        {
            var brand = await _vehicleBrandService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleBrandRequest request)
        {
            var updated = await _vehicleBrandService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _vehicleBrandService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

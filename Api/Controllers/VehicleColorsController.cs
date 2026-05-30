using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.VehicleColors;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
    public sealed class VehicleColorsController : ControllerBase
    {
        private readonly IVehicleColorService _vehicleColorService;

        public VehicleColorsController(IVehicleColorService vehicleColorService)
        {
            _vehicleColorService = vehicleColorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _vehicleColorService.GetAllAsync();
            return Ok(colors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _vehicleColorService.GetByIdAsync(id);
            if (color is null)
                return NotFound();

            return Ok(color);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleColorRequest request)
        {
            var color = await _vehicleColorService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = color.Id }, color);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleColorRequest request)
        {
            var updated = await _vehicleColorService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _vehicleColorService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
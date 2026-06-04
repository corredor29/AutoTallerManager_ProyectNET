using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.VehicleOwnershipHistory;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo VehicleOwnershipHistory.
    public sealed class VehicleOwnershipHistoryController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IVehicleOwnershipHistoryService _ownershipService;

        public VehicleOwnershipHistoryController(IVehicleOwnershipHistoryService ownershipService)
        {
            _ownershipService = ownershipService;
        }

    // Obtiene los registros asociados a un vehiculo especifico.
        [HttpGet("vehicle/{vehicleId:int}")]
        public async Task<IActionResult> GetByVehicleId(int vehicleId)
        {
            var ownerships = await _ownershipService.GetByVehicleIdAsync(vehicleId);
            return Ok(ownerships);
        }

    // Obtiene los registros asociados a un cliente especifico.
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var ownerships = await _ownershipService.GetByCustomerIdAsync(customerId);
            return Ok(ownerships);
        }

    // Devuelve el propietario actual registrado para el vehiculo.
        [HttpGet("vehicle/{vehicleId:int}/current")]
        public async Task<IActionResult> GetCurrentOwner(int vehicleId)
        {
            var ownership = await _ownershipService.GetCurrentOwnerAsync(vehicleId);
            if (ownership is null)
                return NotFound();

            return Ok(ownership);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreateOwnershipRequest request)
        {
            var ownership = await _ownershipService.CreateAsync(request);
            return Ok(ownership);
        }

    // Cierra la vigencia del registro de propiedad indicado.
        [HttpPut("{id:int}/close")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Close(int id, [FromBody] CloseOwnershipRequest request)
        {
            var closed = await _ownershipService.CloseAsync(id, request);
            if (!closed)
                return NotFound();

            return NoContent();
        }
    }
}

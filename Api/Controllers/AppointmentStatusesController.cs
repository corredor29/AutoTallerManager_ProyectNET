using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.AppointmentStatuses;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo AppointmentStatuses.
    public sealed class AppointmentStatusesController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IAppointmentStatusService _appointmentStatusService;

        public AppointmentStatusesController(IAppointmentStatusService appointmentStatusService)
        {
            _appointmentStatusService = appointmentStatusService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var statuses = await _appointmentStatusService.GetAllAsync();
            return Ok(statuses);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var status = await _appointmentStatusService.GetByIdAsync(id);
            if (status is null)
                return NotFound();

            return Ok(status);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentStatusRequest request)
        {
            var status = await _appointmentStatusService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var updated = await _appointmentStatusService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _appointmentStatusService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

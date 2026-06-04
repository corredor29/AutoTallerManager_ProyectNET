using Application.Contracts.Services;
using Application.Requests.Appointments;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo Appointments.
public sealed class AppointmentsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment is null)
        {
            return NotFound();
        }

        return Ok(appointment);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var appointment = await _appointmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var updated = await _appointmentService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Cambia el estado del registro usando una solicitud especifica.
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeAppointmentStatusRequest request)
    {
        var updated = await _appointmentService.ChangeStatusAsync(id, request);
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
        var removed = await _appointmentService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

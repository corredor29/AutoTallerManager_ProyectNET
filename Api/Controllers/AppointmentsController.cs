using Application.Contracts.Services;
using Application.Requests.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var appointment = await _appointmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var updated = await _appointmentService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeAppointmentStatusRequest request)
    {
        var updated = await _appointmentService.ChangeStatusAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
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

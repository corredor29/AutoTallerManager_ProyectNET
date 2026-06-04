using Application.Contracts.Services;
using Application.Requests.QuotationStatuses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo QuotationStatuses.
public sealed class QuotationStatusesController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IQuotationStatusService _quotationStatusService;

    public QuotationStatusesController(IQuotationStatusService quotationStatusService)
    {
        _quotationStatusService = quotationStatusService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _quotationStatusService.GetAllAsync();
        return Ok(statuses);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _quotationStatusService.GetByIdAsync(id);
        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationStatusRequest request)
    {
        var status = await _quotationStatusService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationStatusRequest request)
    {
        var updated = await _quotationStatusService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Elimina el registro indicado si existe.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _quotationStatusService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

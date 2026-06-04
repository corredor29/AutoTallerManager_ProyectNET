using Application.Contracts.Services;
using Application.Requests.Quotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo Quotations.
public sealed class QuotationsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IQuotationService _quotationService;

    public QuotationsController(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var quotations = await _quotationService.GetAllAsync();
        return Ok(quotations);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var quotation = await _quotationService.GetByIdAsync(id);
        if (quotation is null)
        {
            return NotFound();
        }

        return Ok(quotation);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationRequest request)
    {
        var quotation = await _quotationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = quotation.Id }, quotation);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationRequest request)
    {
        var updated = await _quotationService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Cambia el estado del registro usando una solicitud especifica.
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeQuotationStatusRequest request)
    {
        var updated = await _quotationService.ChangeStatusAsync(id, request);
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
        var removed = await _quotationService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

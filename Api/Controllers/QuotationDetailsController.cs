using Application.Contracts.Services;
using Application.Requests.QuotationDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo QuotationDetails.
public sealed class QuotationDetailsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IQuotationDetailService _quotationDetailService;

    public QuotationDetailsController(IQuotationDetailService quotationDetailService)
    {
        _quotationDetailService = quotationDetailService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _quotationDetailService.GetAllAsync();
        return Ok(details);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _quotationDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationDetailRequest request)
    {
        var detail = await _quotationDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationDetailRequest request)
    {
        var updated = await _quotationDetailService.UpdateAsync(id, request);
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
        var removed = await _quotationDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

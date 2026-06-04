using Application.Contracts.Services;
using Application.Requests.InvoiceDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo InvoiceDetails.
public sealed class InvoiceDetailsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IInvoiceDetailService _invoiceDetailService;

    public InvoiceDetailsController(IInvoiceDetailService invoiceDetailService)
    {
        _invoiceDetailService = invoiceDetailService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _invoiceDetailService.GetAllAsync();
        return Ok(details);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _invoiceDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDetailRequest request)
    {
        var detail = await _invoiceDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceDetailRequest request)
    {
        var updated = await _invoiceDetailService.UpdateAsync(id, request);
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
        var removed = await _invoiceDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

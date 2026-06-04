using Api.Security;
using Application.Common.Pagination;
using Application.Contracts.Services;
using Application.Filters;
using Application.Requests.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo Invoices.
public sealed class InvoicesController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] InvoiceFilter filter)
    {
        var result = await _invoiceService.GetAllPagedAsync(pagination, filter);
        Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
        return Ok(result);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        var invoice = await _invoiceService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceRequest request)
    {
        var updated = await _invoiceService.UpdateAsync(id, request);
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
        var removed = await _invoiceService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

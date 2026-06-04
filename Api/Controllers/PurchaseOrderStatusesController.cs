using Application.Contracts.Services;
using Application.Requests.PurchaseOrderStatuses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo PurchaseOrderStatuses.
public sealed class PurchaseOrderStatusesController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;

    public PurchaseOrderStatusesController(IPurchaseOrderStatusService purchaseOrderStatusService)
    {
        _purchaseOrderStatusService = purchaseOrderStatusService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _purchaseOrderStatusService.GetAllAsync();
        return Ok(statuses);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _purchaseOrderStatusService.GetByIdAsync(id);
        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderStatusRequest request)
    {
        var status = await _purchaseOrderStatusService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderStatusRequest request)
    {
        var updated = await _purchaseOrderStatusService.UpdateAsync(id, request);
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
        var removed = await _purchaseOrderStatusService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

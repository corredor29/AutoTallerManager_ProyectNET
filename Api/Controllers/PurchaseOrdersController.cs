using Application.Contracts.Services;
using Application.Requests.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo PurchaseOrders.
public sealed class PurchaseOrdersController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchaseOrders = await _purchaseOrderService.GetAllAsync();
        return Ok(purchaseOrders);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchaseOrder = await _purchaseOrderService.GetByIdAsync(id);
        if (purchaseOrder is null)
        {
            return NotFound();
        }

        return Ok(purchaseOrder);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
    {
        var purchaseOrder = await _purchaseOrderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = purchaseOrder.Id }, purchaseOrder);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderRequest request)
    {
        var updated = await _purchaseOrderService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Cambia el estado del registro usando una solicitud especifica.
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangePurchaseOrderStatusRequest request)
    {
        var updated = await _purchaseOrderService.ChangeStatusAsync(id, request);
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
        var removed = await _purchaseOrderService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

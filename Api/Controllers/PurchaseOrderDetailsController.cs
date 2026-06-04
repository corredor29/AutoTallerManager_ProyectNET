using Application.Contracts.Services;
using Application.Requests.PurchaseOrderDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo PurchaseOrderDetails.
public sealed class PurchaseOrderDetailsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;

    public PurchaseOrderDetailsController(IPurchaseOrderDetailService purchaseOrderDetailService)
    {
        _purchaseOrderDetailService = purchaseOrderDetailService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _purchaseOrderDetailService.GetAllAsync();
        return Ok(details);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _purchaseOrderDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDetailRequest request)
    {
        var detail = await _purchaseOrderDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderDetailRequest request)
    {
        var updated = await _purchaseOrderDetailService.UpdateAsync(id, request);
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
        var removed = await _purchaseOrderDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

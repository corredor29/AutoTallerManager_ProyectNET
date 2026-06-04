using Application.Contracts.Services;
using Application.Requests.PartSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo PartSuppliers.
public sealed class PartSuppliersController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPartSupplierService _partSupplierService;

    public PartSuppliersController(IPartSupplierService partSupplierService)
    {
        _partSupplierService = partSupplierService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _partSupplierService.GetAllAsync();
        return Ok(items);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _partSupplierService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartSupplierRequest request)
    {
        var item = await _partSupplierService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartSupplierRequest request)
    {
        var updated = await _partSupplierService.UpdateAsync(id, request);
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
        var removed = await _partSupplierService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

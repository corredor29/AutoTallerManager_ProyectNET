using Application.Contracts.Services;
using Application.Requests.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo Suppliers.
public sealed class SuppliersController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = await _supplierService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierRequest request)
    {
        var updated = await _supplierService.UpdateAsync(id, request);
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
        var removed = await _supplierService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

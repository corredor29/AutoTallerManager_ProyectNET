using Application.Contracts.Services;
using Application.Requests.ServiceOrderParts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo ServiceOrderParts.
public sealed class ServiceOrderPartsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IServiceOrderPartService _serviceOrderPartService;

    public ServiceOrderPartsController(IServiceOrderPartService serviceOrderPartService)
    {
        _serviceOrderPartService = serviceOrderPartService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _serviceOrderPartService.GetAllAsync();
        return Ok(items);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _serviceOrderPartService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderPartRequest request)
    {
        var item = await _serviceOrderPartService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceOrderPartRequest request)
    {
        var updated = await _serviceOrderPartService.UpdateAsync(id, request);
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
        var removed = await _serviceOrderPartService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

using Application.Contracts.Services;
using Application.Requests.ServiceTypes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo ServiceTypes.
public sealed class ServiceTypesController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IServiceTypeService _serviceTypeService;

    public ServiceTypesController(IServiceTypeService serviceTypeService)
    {
        _serviceTypeService = serviceTypeService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var serviceTypes = await _serviceTypeService.GetAllAsync();
        return Ok(serviceTypes);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceType = await _serviceTypeService.GetByIdAsync(id);
        if (serviceType is null)
        {
            return NotFound();
        }

        return Ok(serviceType);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceTypeRequest request)
    {
        var serviceType = await _serviceTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = serviceType.Id }, serviceType);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceTypeRequest request)
    {
        var updated = await _serviceTypeService.UpdateAsync(id, request);
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
        var removed = await _serviceTypeService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

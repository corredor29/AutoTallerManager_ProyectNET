using Application.Contracts.Services;
using Application.Requests.MeasurementUnits;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo MeasurementUnits.
public sealed class MeasurementUnitsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IMeasurementUnitService _measurementUnitService;

    public MeasurementUnitsController(IMeasurementUnitService measurementUnitService)
    {
        _measurementUnitService = measurementUnitService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _measurementUnitService.GetAllAsync();
        return Ok(units);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await _measurementUnitService.GetByIdAsync(id);
        if (unit is null)
        {
            return NotFound();
        }

        return Ok(unit);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementUnitRequest request)
    {
        var unit = await _measurementUnitService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMeasurementUnitRequest request)
    {
        var updated = await _measurementUnitService.UpdateAsync(id, request);
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
        var removed = await _measurementUnitService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

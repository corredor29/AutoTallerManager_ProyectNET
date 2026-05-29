using Application.Contracts.Services;
using Application.Requests.MeasurementUnits;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MeasurementUnitsController : ControllerBase
{
    private readonly IMeasurementUnitService _measurementUnitService;

    public MeasurementUnitsController(IMeasurementUnitService measurementUnitService)
    {
        _measurementUnitService = measurementUnitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _measurementUnitService.GetAllAsync();
        return Ok(units);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementUnitRequest request)
    {
        var unit = await _measurementUnitService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
    }

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

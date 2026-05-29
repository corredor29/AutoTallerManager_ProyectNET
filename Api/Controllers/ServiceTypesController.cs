using Application.Contracts.Services;
using Application.Requests.ServiceTypes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ServiceTypesController : ControllerBase
{
    private readonly IServiceTypeService _serviceTypeService;

    public ServiceTypesController(IServiceTypeService serviceTypeService)
    {
        _serviceTypeService = serviceTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var serviceTypes = await _serviceTypeService.GetAllAsync();
        return Ok(serviceTypes);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceTypeRequest request)
    {
        var serviceType = await _serviceTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = serviceType.Id }, serviceType);
    }

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

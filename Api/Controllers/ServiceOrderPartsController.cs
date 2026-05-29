using Application.Contracts.Services;
using Application.Requests.ServiceOrderParts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ServiceOrderPartsController : ControllerBase
{
    private readonly IServiceOrderPartService _serviceOrderPartService;

    public ServiceOrderPartsController(IServiceOrderPartService serviceOrderPartService)
    {
        _serviceOrderPartService = serviceOrderPartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _serviceOrderPartService.GetAllAsync();
        return Ok(items);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderPartRequest request)
    {
        var item = await _serviceOrderPartService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

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

using Application.Contracts.Services;
using Application.Requests.PartSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PartSuppliersController : ControllerBase
{
    private readonly IPartSupplierService _partSupplierService;

    public PartSuppliersController(IPartSupplierService partSupplierService)
    {
        _partSupplierService = partSupplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _partSupplierService.GetAllAsync();
        return Ok(items);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartSupplierRequest request)
    {
        var item = await _partSupplierService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

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

using Application.Contracts.Services;
using Application.Requests.PurchaseOrderStatuses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PurchaseOrderStatusesController : ControllerBase
{
    private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;

    public PurchaseOrderStatusesController(IPurchaseOrderStatusService purchaseOrderStatusService)
    {
        _purchaseOrderStatusService = purchaseOrderStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _purchaseOrderStatusService.GetAllAsync();
        return Ok(statuses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _purchaseOrderStatusService.GetByIdAsync(id);
        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderStatusRequest request)
    {
        var status = await _purchaseOrderStatusService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderStatusRequest request)
    {
        var updated = await _purchaseOrderStatusService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _purchaseOrderStatusService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

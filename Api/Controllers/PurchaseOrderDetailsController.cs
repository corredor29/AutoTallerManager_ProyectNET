using Application.Contracts.Services;
using Application.Requests.PurchaseOrderDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PurchaseOrderDetailsController : ControllerBase
{
    private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;

    public PurchaseOrderDetailsController(IPurchaseOrderDetailService purchaseOrderDetailService)
    {
        _purchaseOrderDetailService = purchaseOrderDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _purchaseOrderDetailService.GetAllAsync();
        return Ok(details);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _purchaseOrderDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDetailRequest request)
    {
        var detail = await _purchaseOrderDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderDetailRequest request)
    {
        var updated = await _purchaseOrderDetailService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _purchaseOrderDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

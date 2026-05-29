using Application.Contracts.Services;
using Application.Requests.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchaseOrders = await _purchaseOrderService.GetAllAsync();
        return Ok(purchaseOrders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchaseOrder = await _purchaseOrderService.GetByIdAsync(id);
        if (purchaseOrder is null)
        {
            return NotFound();
        }

        return Ok(purchaseOrder);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
    {
        var purchaseOrder = await _purchaseOrderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = purchaseOrder.Id }, purchaseOrder);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderRequest request)
    {
        var updated = await _purchaseOrderService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangePurchaseOrderStatusRequest request)
    {
        var updated = await _purchaseOrderService.ChangeStatusAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _purchaseOrderService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

using Application.Contracts.Services;
using Application.Requests.ServiceOrders;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Staff)]
[EnableRateLimiting("service-orders")]
public sealed class ServiceOrdersController : ControllerBase
{
    private readonly IServiceOrderService _serviceOrderService;

    public ServiceOrdersController(IServiceOrderService serviceOrderService)
    {
        _serviceOrderService = serviceOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetServiceOrdersRequest request)
    {
        var orders = await _serviceOrderService.GetAllAsync(request);
        Response.Headers.Append("X-Total-Count", orders.TotalCount.ToString());
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _serviceOrderService.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderRequest request)
    {
        var order = await _serviceOrderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.AdminOrMechanic)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceOrderRequest request)
    {
        var updated = await _serviceOrderService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = AppRoles.AdminOrMechanic)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeServiceOrderStatusRequest request)
    {
        var updated = await _serviceOrderService.ChangeStatusAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _serviceOrderService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

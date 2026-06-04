using Api.Security;
using Application.Common.Pagination;
using Application.Contracts.Services;
using Application.Filters;
using Application.Requests.ServiceOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Staff)]
[EnableRateLimiting("service-orders")]
// Controlador que expone los endpoints principales del modulo ServiceOrders.
public sealed class ServiceOrdersController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IServiceOrderService _serviceOrderService;

    public ServiceOrdersController(IServiceOrderService serviceOrderService)
    {
        _serviceOrderService = serviceOrderService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] ServiceOrderFilter filter)
    {
        var result = await _serviceOrderService.GetAllPagedAsync(pagination, filter);
        Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
        return Ok(result);
    }

    // Busca un registro puntual por su identificador.
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

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderRequest request)
    {
        var order = await _serviceOrderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // Actualiza un registro existente identificado por su id.
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

    // Cambia el estado del registro usando una solicitud especifica.
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

    // Elimina el registro indicado si existe.
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

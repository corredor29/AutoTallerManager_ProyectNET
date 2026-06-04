using Api.Security;
using Application.Common.Pagination;
using Application.Contracts.Services;
using Application.Filters;
using Application.Requests.Parts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Staff)]
[EnableRateLimiting("parts")]
// Controlador que expone los endpoints principales del modulo Parts.
public sealed class PartsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] PartFilter filter)
    {
        var result = await _partService.GetAllPagedAsync(pagination, filter);
        Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
        return Ok(result);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var part = await _partService.GetByIdAsync(id);
        if (part is null)
        {
            return NotFound();
        }

        return Ok(part);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreatePartRequest request)
    {
        var part = await _partService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = part.Id }, part);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartRequest request)
    {
        var updated = await _partService.UpdateAsync(id, request);
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
        var removed = await _partService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

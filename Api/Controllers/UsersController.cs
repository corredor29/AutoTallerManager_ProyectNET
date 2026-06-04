using Api.Security;
using Application.Contracts.Services;
using Application.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo Users.
public sealed class UsersController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

// Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

// Devuelve solo mecanicos activos para pantallas de asignacion.
    [HttpGet("mechanics")]
    [Authorize(Roles = AppRoles.Staff)]
    public async Task<IActionResult> GetMechanics()
    {
        var users = await _userService.GetActiveMechanicsAsync();
        return Ok(users);
    }

// Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

// Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

// Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var updated = await _userService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

// Activa el usuario para permitir nuevamente su uso en el sistema.
    [HttpPut("{id:int}/activate")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Activate(int id)
    {
        var updated = await _userService.ActivateAsync(id);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

// Desactiva el usuario para bloquear su uso sin borrarlo.
    [HttpPut("{id:int}/deactivate")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var updated = await _userService.DeactivateAsync(id);
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
        var removed = await _userService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

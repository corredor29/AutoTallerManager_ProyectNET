using Application.Contracts.Services;
using Application.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    // Servicio que concentra la lógica de login y registro.
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Permite iniciar sesión sin requerir un token previo.
    [AllowAnonymous]
    [HttpPost("login")]
    // Protege este endpoint contra intentos masivos de autenticación.
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
    // Permite registrar usuarios nuevos sin autenticación previa.
    [AllowAnonymous]
    [HttpPost("register")]
    // Comparte la misma política de límite para evitar abuso del registro.
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }
}

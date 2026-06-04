using Application.Contracts.Services;
using Application.Requests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

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

    // Inicia el flujo OAuth con Google redirigiendo al proveedor externo.
    [AllowAnonymous]
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        // Redirige al usuario a Google para autenticarse.
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
        var properties  = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // Google redirige aquí tras la autenticación con los datos del usuario.
    [AllowAnonymous]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        // Google almacena la identidad en la cookie antes de llegar aquí.
        var authenticateResult = await HttpContext.AuthenticateAsync("Cookies");

        if (!authenticateResult.Succeeded)
        {
            // Devuelve el mensaje real del error para facilitar el diagnóstico.
            var failure = authenticateResult.Failure?.Message ?? "Unknown error";
            return BadRequest($"Google authentication failed: {failure}");
        }

        var claims = authenticateResult.Principal?.Claims;
        if (claims is null)
            return BadRequest("No claims received from Google.");

        // Extrae los datos esenciales del perfil de Google.
        var email     = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var lastName  = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email not provided by Google.");

        // Delega al servicio la lógica de login o registro con Google.
        var result = await _authService.LoginWithGoogleAsync(new GoogleLoginRequest
        {
            Email     = email,
            FirstName = firstName ?? "Google",
            LastName  = lastName  ?? "User"
        });

        // Redirige al frontend con el token en la URL para que lo guarde.
        var frontendUrl = $"http://127.0.0.1:5500/index.html?token={result.Token}" +
                          $"&fullName={Uri.EscapeDataString(result.FullName)}" +
                          $"&roles={Uri.EscapeDataString(string.Join(",", result.Roles))}" +
                          $"&userId={result.UserId}";

        return Redirect(frontendUrl);
    }

    // Valida el ID token que Google emite directamente en el frontend (Google Identity Services).
    [AllowAnonymous]
    [HttpPost("google-token")]
    // Comparte el límite de auth para evitar abuso del endpoint de Google.
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> GoogleToken([FromBody] GoogleTokenRequest request)
    {
        var result = await _authService.LoginWithGoogleTokenAsync(request.IdToken);
        return Ok(result);
    }
}
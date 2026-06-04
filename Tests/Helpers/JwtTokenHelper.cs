using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AutoTallerManager.Tests.Helpers;

internal static class JwtTokenHelper
{
    private const string Key      = "AutoTallerManager-SuperSecret-Key-Change-This-In-Production-2026";
    private const string Issuer   = "AutoTallerManager.Api";
    private const string Audience = "AutoTallerManager.Client";

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    internal static string GenerateToken(int userId = 1, params string[] roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new("personId",                         "1"),
            new(JwtRegisteredClaimNames.GivenName,  "Test"),
            new(JwtRegisteredClaimNames.FamilyName, "User"),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer:             Issuer,
            audience:           Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    internal static string AdminToken        => GenerateToken(1, "Admin", "Mechanic", "Receptionist");
    internal static string MechanicToken     => GenerateToken(1, "Mechanic");
    internal static string ReceptionistToken => GenerateToken(1, "Receptionist");
}

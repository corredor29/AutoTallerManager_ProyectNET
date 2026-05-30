using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Auth;
using Application.Requests.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
    {
        var emailParts = request.Email
            .Trim()
            .ToLowerInvariant()
            .Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (emailParts.Length != 2)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var user = await _userRepository.GetByPrimaryEmailAsync(emailParts[0], emailParts[1]);
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash.Value))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var roles = user.UserRoles
            .Select(x => x.Role.RoleName.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
            new("personId", user.PersonId.ToString()),
            new(JwtRegisteredClaimNames.GivenName, user.Person.FirstName.Value),
            new(JwtRegisteredClaimNames.FamilyName, user.Person.LastName.Value)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(descriptor);

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            PersonId = user.PersonId,
            FullName = $"{user.Person.FirstName.Value} {user.Person.LastName.Value}".Trim(),
            Roles = roles
        };
    }
}

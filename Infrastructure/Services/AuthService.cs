using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Auth;
using Application.Requests.Auth;
using Domain.Entities.Persons;
using Domain.Entities.Users;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Persons.PersonEmail;
using Domain.ValueObject.Users.User;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{

    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository    _userRepository;
        private readonly JwtOptions         _jwtOptions;
        private readonly AutoTallerDbContext _dbContext;

        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtOptions> jwtOptions,
            AutoTallerDbContext dbContext)
        {
            _userRepository = userRepository;
            _jwtOptions     = jwtOptions.Value;
            _dbContext      = dbContext;
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

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
        {
            var emailParts  = request.Email.Trim().ToLowerInvariant()
                                .Split('@', StringSplitOptions.RemoveEmptyEntries);
            var emailUser   = emailParts[0];
            var emailDomain = emailParts[1];

            var allDomains   = await _dbContext.EmailDomains.ToListAsync();
            var domainEntity = allDomains.FirstOrDefault(d => d.Domain.Value == emailDomain);

            if (domainEntity is null)
            {
                domainEntity = new EmailDomain(new EmailDomainValue(emailDomain));
                await _dbContext.EmailDomains.AddAsync(domainEntity);
                await _dbContext.SaveChangesAsync();
            }

            var allEmails  = await _dbContext.PersonEmails.ToListAsync();
            var emailExists = allEmails.Any(e => e.EmailUser.Value == emailUser
                                            && e.EmailDomainId == domainEntity.Id);

            if (emailExists)
                throw new InvalidOperationException("Email already registered.");

            var person = new Person(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName)
            );

            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            var personEmail = new PersonEmail(
                person.Id,
                domainEntity.Id,
                new EmailUser(emailUser),
                true
            );

            await _dbContext.PersonEmails.AddAsync(personEmail);

            var passwordHash = new PasswordHash(BCrypt.Net.BCrypt.HashPassword(request.Password));
            var user = new User(person.Id, passwordHash);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var allRoles = await _dbContext.Roles.ToListAsync();
            var receptionistRole = allRoles.FirstOrDefault(r => r.RoleName.Value == "Receptionist")
                ?? throw new InvalidOperationException("Receptionist role not found.");

            var userRole = new UserRole(user.Id, receptionistRole.Id);
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            return await LoginAsync(new LoginRequest
            {
                Email    = request.Email,
                Password = request.Password
            });
        }
    }
}

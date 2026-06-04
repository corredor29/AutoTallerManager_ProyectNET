using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Auth;
using Application.DTOs.Notifications;
using Application.Requests.Auth;
using Domain.Entities.Persons;
using Domain.Entities.Users;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Persons.PersonEmail;
using Domain.ValueObject.Users.User;
using Infrastructure.Context;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public sealed class AuthService : IAuthService
    {
        // Repositorio especializado para consultar usuarios con sus relaciones cargadas.
        private readonly IUserRepository             _userRepository;
        // Configuración JWT usada para firmar y expirar tokens.
        private readonly JwtOptions                  _jwtOptions;
        // DbContext requerido para crear registros relacionados durante el registro.
        private readonly AutoTallerDbContext          _dbContext;
        // Hub usado para enviar notificaciones en tiempo real al frontend.
        private readonly IHubContext<NotificationHub> _hub;

        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtOptions> jwtOptions,
            AutoTallerDbContext dbContext,
            IHubContext<NotificationHub> hub)
        {
            _userRepository = userRepository;
            _jwtOptions     = jwtOptions.Value;
            _dbContext      = dbContext;
            _hub            = hub;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
        {
            // Normaliza el email y lo divide para buscarlo según el modelo usuario/dominio.
            var emailParts = request.Email
                .Trim()
                .ToLowerInvariant()
                .Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Si el correo no tiene el formato esperado, se rechaza la autenticación.
            if (emailParts.Length != 2)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Busca el usuario por su correo principal y valida que esté activo.
            var user = await _userRepository.GetByPrimaryEmailAsync(emailParts[0], emailParts[1]);
            if (user is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // La contraseña enviada se compara contra el hash almacenado.
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash.Value))
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Se recopilan los roles para incluirlos en autorización y respuesta.
            var roles = user.UserRoles
                .Select(x => x.Role.RoleName.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Calcula el vencimiento y prepara las utilidades del token.
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            // Claims base que identificarán al usuario autenticado dentro del sistema.
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                new("personId",                         user.PersonId.ToString()),
                new(JwtRegisteredClaimNames.GivenName,  user.Person.FirstName.Value),
                new(JwtRegisteredClaimNames.FamilyName, user.Person.LastName.Value)
            };

            // Cada rol también viaja como claim para que la autorización por roles funcione.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Aquí se define exactamente cómo quedará construido el JWT.
            var descriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = expiresAtUtc,
                Issuer             = _jwtOptions.Issuer,
                Audience           = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            // Se genera el token y un nombre amigable para devolver al cliente.
            var token    = tokenHandler.CreateToken(descriptor);
            var fullName = $"{user.Person.FirstName.Value} {user.Person.LastName.Value}".Trim();

            // Notifica a todos los clientes conectados que alguien inició sesión.
            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "login",
                Entity     = "User",
                RecordId   = user.Id,
                Message    = $"{fullName} logged in",
                OccurredAt = DateTime.UtcNow
            });

            return new AuthResponseDto
            {
                Token        = tokenHandler.WriteToken(token),
                ExpiresAtUtc = expiresAtUtc,
                UserId       = user.Id,
                PersonId     = user.PersonId,
                FullName     = fullName,
                Roles        = roles
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
        {
            // Divide el correo para persistirlo siguiendo el modelo del dominio.
            var emailParts  = request.Email.Trim().ToLowerInvariant()
                                .Split('@', StringSplitOptions.RemoveEmptyEntries);
            var emailUser   = emailParts[0];
            var emailDomain = emailParts[1];

            // Intenta reutilizar un dominio existente antes de crear uno nuevo.
            var allDomains   = await _dbContext.EmailDomains.ToListAsync();
            var domainEntity = allDomains.FirstOrDefault(d => d.Domain.Value == emailDomain);

            if (domainEntity is null)
            {
                // Si el dominio no existe todavía, se crea una vez y queda disponible.
                domainEntity = new EmailDomain(new EmailDomainValue(emailDomain));
                await _dbContext.EmailDomains.AddAsync(domainEntity);
                await _dbContext.SaveChangesAsync();
            }

            // Evita registrar dos veces el mismo correo.
            var allEmails   = await _dbContext.PersonEmails.ToListAsync();
            var emailExists = allEmails.Any(e =>
                e.EmailUser.Value == emailUser && e.EmailDomainId == domainEntity.Id);

            if (emailExists)
                throw new InvalidOperationException("Email already registered.");

            // La persona se crea primero porque el usuario depende de ese registro.
            var person = new Person(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName));

            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            // Este registro marca cuál es el email asociado a la persona.
            var personEmail = new PersonEmail(
                person.Id, domainEntity.Id, new EmailUser(emailUser), true);

            await _dbContext.PersonEmails.AddAsync(personEmail);

            // La contraseña se almacena como hash, nunca en texto plano.
            var passwordHash = new PasswordHash(BCrypt.Net.BCrypt.HashPassword(request.Password));
            var user         = new User(person.Id, passwordHash);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // ── Asignar rol según request, fallback a Receptionist ──
            var allRoles     = await _dbContext.Roles.ToListAsync();
            // Si el cliente no envía rol, se asigna Receptionist como valor por defecto.
            var roleName     = string.IsNullOrWhiteSpace(request.Role) ? "Receptionist" : request.Role.Trim();
            var assignedRole = allRoles.FirstOrDefault(r => r.RoleName.Value == roleName)
                ?? allRoles.FirstOrDefault(r => r.RoleName.Value == "Receptionist")
                ?? throw new InvalidOperationException("Role not found.");

            // Guarda la relación entre el nuevo usuario y su rol.
            var userRole = new UserRole(user.Id, assignedRole.Id);
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            // Publica una notificación para que el frontend refleje el nuevo registro.
            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "create",
                Entity     = "User",
                RecordId   = user.Id,
                Message    = $"New user {request.FirstName} {request.LastName} registered as {assignedRole.RoleName.Value}",
                OccurredAt = DateTime.UtcNow
            });

            // Reutiliza el flujo de login para devolver el token listo al usuario nuevo.
            return await LoginAsync(new LoginRequest
            {
                Email    = request.Email,
                Password = request.Password
            });
        }
    }
}

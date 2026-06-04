using Application.Requests.Auth;
using AutoTallerManager.Tests.Fakes;
using AutoTallerManager.Tests.Helpers;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class AuthServiceTests
{
    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task LoginAsync_ReturnsTokenAndRoles_WhenCredentialsAreValid()
    {
        var user = TestEntityBuilder.BuildUser(roles: ["Admin", "Mechanic"]);
        var repository = new FakeUserRepository(user);
        var service = CreateService(repository);

        var response = await service.LoginAsync(new LoginRequest
        {
            Email = "admin@autotaller.local",
            Password = "Admin123!"
        });

        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(user.PersonId, response.PersonId);
        Assert.Equal("Jane Doe", response.FullName);
        Assert.Contains("Admin", response.Roles);
        Assert.Contains("Mechanic", response.Roles);
        Assert.True(response.ExpiresAtUtc > DateTime.UtcNow);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository(null);
        var service = CreateService(repository);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest
            {
                Email = "missing@autotaller.local",
                Password = "Admin123!"
            }));

        Assert.Equal("Invalid credentials.", exception.Message);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenPasswordIsInvalid()
    {
        var user = TestEntityBuilder.BuildUser(password: "Different123!");
        var repository = new FakeUserRepository(user);
        var service = CreateService(repository);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest
            {
                Email = "admin@autotaller.local",
                Password = "WrongPassword1!"
            }));

        Assert.Equal("Invalid credentials.", exception.Message);
    }

    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static AuthService CreateService(FakeUserRepository repository)
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer            = "tests",
            Audience          = "tests",
            Key               = "Tests-Super-Secret-Key-For-Jwt-Generation-2026",
            ExpirationMinutes = 60
        });

        var dbContextOptions = new DbContextOptionsBuilder<AutoTallerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new AutoTallerDbContext(
            dbContextOptions,
            new HttpContextAccessor()
        );

        return new AuthService(repository, jwtOptions, dbContext);
    }
}

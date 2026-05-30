using Application.Requests.Auth;
using AutoTallerManager.Tests.Fakes;
using AutoTallerManager.Tests.Helpers;
using Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class AuthServiceTests
{
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

    private static AuthService CreateService(FakeUserRepository repository)
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "tests",
            Audience = "tests",
            Key = "Tests-Super-Secret-Key-For-Jwt-Generation-2026",
            ExpirationMinutes = 60
        });

        return new AuthService(repository, options);
    }
}

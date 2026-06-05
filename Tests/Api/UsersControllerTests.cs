using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoTallerManager.Tests.Helpers;
using Domain.Entities.Persons;
using Domain.Entities.Users;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Users.Role;
using Domain.ValueObject.Users.User;

namespace AutoTallerManager.Tests.Api;

public sealed class UsersControllerTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    private readonly TestWebAppFactory _factory;

    public UsersControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedAsync(async db =>
        {
            if (await db.Users.AnyAsync(x => x.UserRoles.Any(ur => ur.Role.RoleName.Value == "Mechanic")))
                return;

            var person = new Person(new PersonFirstName("Carlos"), new PersonLastName("Mechanic"));
            await db.Persons.AddAsync(person);
            await db.SaveChangesAsync();

            var user = new User(person.Id, new PasswordHash(BCrypt.Net.BCrypt.HashPassword("Pass123!")));
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var mechanicRole = await db.Roles.FirstAsync(x => x.RoleName.Value == "Mechanic");
            await db.UserRoles.AddAsync(new UserRole(user.Id, mechanicRole.Id));
            await db.SaveChangesAsync();
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetMechanics_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.GetAsync("/api/users/mechanics");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMechanics_WithReceptionistToken_ReturnsActiveMechanics()
    {
        var client = _factory.CreateReceptionistClient();

        var response = await client.GetAsync("/api/users/mechanics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var items = body.GetProperty("data").EnumerateArray().ToArray();

        items.Should().NotBeEmpty();
        items.Should().OnlyContain(item =>
            item.GetProperty("roles").EnumerateArray().Any(role => role.GetString() == "Mechanic"));
    }
}

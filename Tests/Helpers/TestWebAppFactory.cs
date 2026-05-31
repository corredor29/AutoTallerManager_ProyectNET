using System.Net.Http.Headers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTallerManager.Tests.Helpers;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing Npgsql options registration (avoids dual-provider conflict)
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AutoTallerDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Directly inject InMemory options — do NOT call AddDbContext again
            // (calling AddDbContext twice registers both Npgsql and InMemory provider services)
            var inMemoryOptions = new DbContextOptionsBuilder<AutoTallerDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
            services.AddScoped<DbContextOptions<AutoTallerDbContext>>(_ => inMemoryOptions);
        });
    }

    // Seed catalog data (DatabaseInitializer uses EnsureCreated for InMemory)
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        await db.Database.EnsureCreatedAsync();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    public new Task DisposeAsync() => base.DisposeAsync().AsTask();

    // ── Client helpers ──────────────────────────────────────────────

    public HttpClient CreateAnonymousClient() => CreateClient();

    public HttpClient CreateAuthenticatedClient(string? bearerToken = null)
    {
        var client = CreateClient();
        var token  = bearerToken ?? JwtTokenHelper.AdminToken;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public HttpClient CreateAdminClient()        => CreateAuthenticatedClient(JwtTokenHelper.AdminToken);
    public HttpClient CreateMechanicClient()     => CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);
    public HttpClient CreateReceptionistClient() => CreateAuthenticatedClient(JwtTokenHelper.ReceptionistToken);

    // Access the scoped DbContext for seeding test-specific data
    public async Task SeedAsync(Func<AutoTallerDbContext, Task> seeder)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        await seeder(db);
    }
}

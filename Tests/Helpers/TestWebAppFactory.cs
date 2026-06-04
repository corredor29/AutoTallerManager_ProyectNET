using System.Net.Http.Headers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTallerManager.Tests.Helpers;

// Helper reutilizable para preparar datos o infraestructura de pruebas.
public sealed class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Dependencia compartida por varios escenarios de esta clase de pruebas.
    private readonly string _dbName = Guid.NewGuid().ToString();

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
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
    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        await db.Database.EnsureCreatedAsync();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public new Task DisposeAsync() => base.DisposeAsync().AsTask();

    // ── Client helpers ──────────────────────────────────────────────

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public HttpClient CreateAnonymousClient() => CreateClient();

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public HttpClient CreateAuthenticatedClient(string? bearerToken = null)
    {
        var client = CreateClient();
        var token  = bearerToken ?? JwtTokenHelper.AdminToken;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public HttpClient CreateAdminClient()        => CreateAuthenticatedClient(JwtTokenHelper.AdminToken);
    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public HttpClient CreateMechanicClient()     => CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);
    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public HttpClient CreateReceptionistClient() => CreateAuthenticatedClient(JwtTokenHelper.ReceptionistToken);

    // Access the scoped DbContext for seeding test-specific data
    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public async Task SeedAsync(Func<AutoTallerDbContext, Task> seeder)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        await seeder(db);
    }
}

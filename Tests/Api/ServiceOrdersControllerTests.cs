using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Pagination;
using Application.DTOs.ServiceOrders;

namespace AutoTallerManager.Tests.Api;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class ServiceOrdersControllerTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    // Dependencia compartida por varios escenarios de esta clase de pruebas.
    private readonly TestWebAppFactory _factory;

    public ServiceOrdersControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync()    => Task.CompletedTask;

    // ── GET /api/serviceorders ───────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAll_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.GetAsync("/api/serviceorders");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAll_WithValidToken_Returns200WithPagedResult()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/serviceorders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("data").TryGetProperty("items", out _).Should().BeTrue();
        body.GetProperty("data").TryGetProperty("totalCount", out _).Should().BeTrue();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAll_WithPaginationParams_ReturnsPaginatedResponse()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/serviceorders?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Total-Count");
    }

    // ── GET /api/serviceorders/{id} ──────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetById_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.GetAsync("/api/serviceorders/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetById_NonExistentId_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/serviceorders/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── POST /api/serviceorders ──────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task Post_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("/api/serviceorders", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task Post_WithMechanicToken_Returns403Forbidden()
    {
        // Only AdminOrReceptionist can create orders
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.PostAsJsonAsync("/api/serviceorders", new
        {
            VehicleId     = 1,
            ServiceTypeId = 1,
            MechanicId    = 1,
            OrderStatusId = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── PUT /api/serviceorders/{id}/status ───────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task ChangeStatus_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.PutAsJsonAsync("/api/serviceorders/1/status", new { OrderStatusId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task ChangeStatus_NonExistentOrder_Returns404NotFound()
    {
        var client = _factory.CreateAdminClient();

        var response = await client.PutAsJsonAsync("/api/serviceorders/99999/status",
            new { OrderStatusId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── DELETE /api/serviceorders/{id} ───────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task Delete_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.DeleteAsync("/api/serviceorders/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task Delete_WithMechanicToken_Returns403Forbidden()
    {
        // Only Admin can delete
        var client = _factory.CreateMechanicClient();

        var response = await client.DeleteAsync("/api/serviceorders/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

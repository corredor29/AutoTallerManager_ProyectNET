using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AutoTallerManager.Tests.Api;

public sealed class PartsControllerTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    private readonly TestWebAppFactory _factory;

    public PartsControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync()    => Task.CompletedTask;

    // ── GET /api/parts ───────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.GetAsync("/api/parts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithValidToken_Returns200WithPagedResult()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/parts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("data").TryGetProperty("items", out _).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WithPaginationAndFilter_Returns200WithXTotalCount()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/parts?pageNumber=1&pageSize=10&belowMinStock=false");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Total-Count");
    }

    // ── GET /api/parts/{id} ──────────────────────────────────────────

    [Fact]
    public async Task GetById_NonExistentId_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient(JwtTokenHelper.MechanicToken);

        var response = await client.GetAsync("/api/parts/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── POST /api/parts ──────────────────────────────────────────────

    [Fact]
    public async Task Post_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("/api/parts", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_WithMechanicToken_Returns403Forbidden()
    {
        // Only Admin can create parts
        var client = _factory.CreateMechanicClient();

        var response = await client.PostAsJsonAsync("/api/parts", new
        {
            PartCategoryId = 1,
            Code = "TST-001",
            Description = "Test Part",
            Stock = 10, MinStock = 2,
            UnitPrice = 50.0
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_WithAdminToken_ValidRequest_Returns201()
    {
        // Seed a part category first
        int categoryId = 0;
        await _factory.SeedAsync(async db =>
        {
            var c = await SeedDataHelper.SeedPartCategoryAsync(db, $"Cat-{Guid.NewGuid()}");
            categoryId = c;
        });

        var client = _factory.CreateAdminClient();

        var response = await client.PostAsJsonAsync("/api/parts", new
        {
            PartCategoryId = categoryId,
            Code           = $"INT-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Description    = "Integration test part",
            Stock          = 5,
            MinStock       = 1,
            UnitPrice      = 99.99
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ── PUT /api/parts/{id} ──────────────────────────────────────────

    [Fact]
    public async Task Put_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.PutAsJsonAsync("/api/parts/1", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── DELETE /api/parts/{id} ───────────────────────────────────────

    [Fact]
    public async Task Delete_WithoutToken_Returns401Unauthorized()
    {
        var client = _factory.CreateAnonymousClient();

        var response = await client.DeleteAsync("/api/parts/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_NonExistentId_Returns404NotFound()
    {
        var client = _factory.CreateAdminClient();

        var response = await client.DeleteAsync("/api/parts/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

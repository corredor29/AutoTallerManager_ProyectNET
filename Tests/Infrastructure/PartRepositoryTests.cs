using Application.Common.Pagination;
using Application.Filters;
using Infrastructure.Repositories;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class PartRepositoryTests
{
    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static PartRepository CreateRepository(AutoTallerDbContext db) =>
        new(db);

    // ── GetAllPagedAsync ─────────────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterBelowMinStock_ReturnsOnlyLowStockParts()
    {
        var db         = DbContextFactory.Create();
        var repo       = CreateRepository(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);

        await SeedDataHelper.SeedPartAsync(db, categoryId, "OK-001",  stock: 10, minStock: 2);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "LOW-001", stock: 1,  minStock: 5);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "LOW-002", stock: 0,  minStock: 3);

        var result = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { BelowMinStock = true });

        result.TotalCount.Should().Be(2);
        result.Items.Select(p => p.Code.Value)
            .Should().BeEquivalentTo(["LOW-001", "LOW-002"]);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterByCategory_ReturnsOnlyMatchingCategoryParts()
    {
        var db      = DbContextFactory.Create();
        var repo    = CreateRepository(db);
        var catEngine = await SeedDataHelper.SeedPartCategoryAsync(db, "Engine");
        var catBrakes = await SeedDataHelper.SeedPartCategoryAsync(db, "Brakes");

        await SeedDataHelper.SeedPartAsync(db, catEngine, "E-001");
        await SeedDataHelper.SeedPartAsync(db, catEngine, "E-002");
        await SeedDataHelper.SeedPartAsync(db, catBrakes, "B-001");

        var result = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { PartCategoryId = catEngine });

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(p => p.PartCategoryId == catEngine);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_EmptyFilter_ReturnsPaginatedAllParts()
    {
        var db         = DbContextFactory.Create();
        var repo       = CreateRepository(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);

        for (var i = 1; i <= 15; i++)
            await SeedDataHelper.SeedPartAsync(db, categoryId, $"P-{i:D3}");

        var page1 = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter());

        var page2 = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 2, PageSize = 10 },
            new PartFilter());

        page1.TotalCount.Should().Be(15);
        page1.Items.Should().HaveCount(10);
        page1.HasNextPage.Should().BeTrue();

        page2.Items.Should().HaveCount(5);
        page2.HasPreviousPage.Should().BeTrue();
        page2.HasNextPage.Should().BeFalse();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterIsActive_ReturnsOnlyActiveOrInactiveParts()
    {
        var db         = DbContextFactory.Create();
        var repo       = CreateRepository(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);
        var partId     = await SeedDataHelper.SeedPartAsync(db, categoryId, "ACT-001");

        // Deactivate the part
        var part = await db.Parts.FindAsync(partId);
        part!.Deactivate();
        await db.SaveChangesAsync();

        await SeedDataHelper.SeedPartAsync(db, categoryId, "ACT-002"); // active

        var activeResult = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { IsActive = true });

        var inactiveResult = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { IsActive = false });

        activeResult.TotalCount.Should().Be(1);
        activeResult.Items.Should().ContainSingle(p => p.Code.Value == "ACT-002");

        inactiveResult.TotalCount.Should().Be(1);
        inactiveResult.Items.Should().ContainSingle(p => p.Code.Value == "ACT-001");
    }
}

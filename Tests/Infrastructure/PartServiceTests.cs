using Application.Common.Pagination;
using Application.Filters;
using Application.Requests.Parts;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.Part;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class PartServiceTests
{
    private static PartService CreateService(AutoTallerDbContext db) =>
        new(new PartRepository(db), db);

    // ── CreateAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_UniqueCode_CreatesPartSuccessfully()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);

        var result = await service.CreateAsync(new CreatePartRequest
        {
            PartCategoryId = categoryId,
            Code           = "ENG-001",
            Description    = "Engine filter",
            Stock          = 20,
            MinStock       = 3,
            UnitPrice      = 89.99m
        });

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Code.Should().Be("ENG-001");
        result.Stock.Should().Be(20);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ThrowsInvalidOperationException()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);

        await service.CreateAsync(new CreatePartRequest
        {
            PartCategoryId = categoryId, Code = "DUP-001",
            Description = "First",       Stock = 5, MinStock = 1, UnitPrice = 10m
        });

        var act = () => service.CreateAsync(new CreatePartRequest
        {
            PartCategoryId = categoryId, Code = "DUP-001",  // same code
            Description = "Second",      Stock = 5, MinStock = 1, UnitPrice = 10m
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task CreateAsync_CategoryDoesNotExist_ThrowsArgumentException()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var act = () => service.CreateAsync(new CreatePartRequest
        {
            PartCategoryId = 999,
            Code = "TST-001", Description = "Test", Stock = 1, MinStock = 1, UnitPrice = 1m
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*category*");
    }

    // ── Domain entity: AddStock / RemoveStock ────────────────────────

    [Fact]
    public void AddStock_IncreasesStockCorrectly()
    {
        var part = BuildPart(stock: 5);

        part.AddStock(new PartStock(3));

        part.Stock.Value.Should().Be(8);
    }

    [Fact]
    public void RemoveStock_InsufficientStock_ThrowsInvalidOperationException()
    {
        var part = BuildPart(stock: 2);

        var act = () => part.RemoveStock(new PartStock(5));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task GetAllPagedAsync_FilterByCategory_ReturnsOnlyMatchingParts()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var catA       = await SeedDataHelper.SeedPartCategoryAsync(db, "Category A");
        var catB       = await SeedDataHelper.SeedPartCategoryAsync(db, "Category B");
        await SeedDataHelper.SeedPartAsync(db, catA, "A-001");
        await SeedDataHelper.SeedPartAsync(db, catB, "B-001");

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { PartCategoryId = catA });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(p => p.Code == "A-001");
    }

    [Fact]
    public async Task GetAllPagedAsync_FilterBelowMinStock_ReturnsOnlyLowStockParts()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "OK-001", stock: 10, minStock: 2);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "LOW-001", stock: 1,  minStock: 5);

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { BelowMinStock = true });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(p => p.Code == "LOW-001");
    }

    private static Part BuildPart(int stock = 10) =>
        new(1, new PartCode("TST-001"), new PartDescription("Test"),
            new PartStock(stock), new PartMinStock(2), new PartUnitPrice(50m));
}

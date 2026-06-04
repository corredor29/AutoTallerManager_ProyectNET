using Application.Common.Pagination;
using Application.Filters;
using Application.Requests.Parts;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.Part;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class PartServiceTests
{
    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static PartService CreateService(AutoTallerDbContext db)
    {
        // Mock del hub de SignalR — no necesita hacer nada real en los tests.
        var mockHub = new Mock<IHubContext<NotificationHub>>();
        mockHub
            .Setup(h => h.Clients.All.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new PartService(new PartRepository(db), db, mockHub.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
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
            PartCategoryId = categoryId, Code = "DUP-001",
            Description = "Second",      Stock = 5, MinStock = 1, UnitPrice = 10m
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void AddStock_IncreasesStockCorrectly()
    {
        var part = BuildPart(stock: 5);

        part.AddStock(new PartStock(3));

        part.Stock.Value.Should().Be(8);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void RemoveStock_InsufficientStock_ThrowsInvalidOperationException()
    {
        var part = BuildPart(stock: 2);

        var act = () => part.RemoveStock(new PartStock(5));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Insufficient stock*");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterByCategory_ReturnsOnlyMatchingParts()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var catA    = await SeedDataHelper.SeedPartCategoryAsync(db, "Category A");
        var catB    = await SeedDataHelper.SeedPartCategoryAsync(db, "Category B");
        await SeedDataHelper.SeedPartAsync(db, catA, "A-001");
        await SeedDataHelper.SeedPartAsync(db, catB, "B-001");

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { PartCategoryId = catA });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(p => p.Code == "A-001");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterBelowMinStock_ReturnsOnlyLowStockParts()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var categoryId = await SeedDataHelper.SeedPartCategoryAsync(db);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "OK-001",  stock: 10, minStock: 2);
        await SeedDataHelper.SeedPartAsync(db, categoryId, "LOW-001", stock: 1,  minStock: 5);

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new PartFilter { BelowMinStock = true });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(p => p.Code == "LOW-001");
    }

    // Construye un objeto de prueba con valores validos por defecto.
    private static Part BuildPart(int stock = 10) =>
        new(1, new PartCode("TST-001"), new PartDescription("Test"),
            new PartStock(stock), new PartMinStock(2), new PartUnitPrice(50m));
}
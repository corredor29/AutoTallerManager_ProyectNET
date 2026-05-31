using Application.Common.Pagination;
using Application.Filters;
using Infrastructure.Repositories;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class ServiceOrderRepositoryTests
{
    private static ServiceOrderRepository CreateRepository(AutoTallerDbContext db) =>
        new(db);

    private static async Task<(int vehicleId, int serviceTypeId, int mechanicId, int pendingId, int inProgressId)>
        SeedPrerequisitesAsync(AutoTallerDbContext db)
    {
        var (pending, inProgress, _, _) = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var (diagnostics, _)            = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId)                = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId                   = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var (_, mechanicId)             = await SeedDataHelper.SeedMechanicAsync(db);

        return (vehicleId, diagnostics, mechanicId, pending, inProgress);
    }

    // ── HasActiveOrderForVehicleAsync ────────────────────────────────

    [Fact]
    public async Task HasActiveOrderForVehicleAsync_VehicleHasPendingOrder_ReturnsTrue()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId, _) =
            await SeedPrerequisitesAsync(db);

        await SeedDataHelper.SeedServiceOrderAsync(
            db, vehicleId, serviceTypeId, mechanicId, pendingId);

        var result = await repo.HasActiveOrderForVehicleAsync(vehicleId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasActiveOrderForVehicleAsync_VehicleHasInProgressOrder_ReturnsTrue()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        var (vehicleId, serviceTypeId, mechanicId, _, inProgressId) =
            await SeedPrerequisitesAsync(db);

        await SeedDataHelper.SeedServiceOrderAsync(
            db, vehicleId, serviceTypeId, mechanicId, inProgressId);

        var result = await repo.HasActiveOrderForVehicleAsync(vehicleId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasActiveOrderForVehicleAsync_VehicleHasNoOrders_ReturnsFalse()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        await SeedDataHelper.SeedOrderStatusesAsync(db);

        var result = await repo.HasActiveOrderForVehicleAsync(1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasActiveOrderForVehicleAsync_VehicleHasOnlyCompletedOrder_ReturnsFalse()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        var (vehicleId, serviceTypeId, mechanicId, _, _) =
            await SeedPrerequisitesAsync(db);
        var completedId = (await db.OrderStatuses.ToListAsync())
            .First(s => s.Name.Value == "Completed").Id;

        await SeedDataHelper.SeedServiceOrderAsync(
            db, vehicleId, serviceTypeId, mechanicId, completedId);

        var result = await repo.HasActiveOrderForVehicleAsync(vehicleId);

        result.Should().BeFalse();
    }

    // ── GetAllPagedAsync ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllPagedAsync_MultipleOrders_ReturnsPaginatedResult()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId, _) =
            await SeedPrerequisitesAsync(db);

        // Seed a second vehicle for the second order
        var vehicleId2 = await SeedDataHelper.SeedVehicleAsync(db, 1, "2T1BURHE0JC025234");
        await SeedDataHelper.SeedServiceOrderAsync(db, vehicleId,  serviceTypeId, mechanicId, pendingId);
        await SeedDataHelper.SeedServiceOrderAsync(db, vehicleId2, serviceTypeId, mechanicId, pendingId);

        var result = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 1 },
            new ServiceOrderFilter());

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(1);        // page size = 1
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllPagedAsync_FilterByOrderStatusId_ReturnsOnlyMatchingOrders()
    {
        var db   = DbContextFactory.Create();
        var repo = CreateRepository(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId, inProgressId) =
            await SeedPrerequisitesAsync(db);

        var vehicleId2 = await SeedDataHelper.SeedVehicleAsync(db, 1, "2T1BURHE0JC025234");
        await SeedDataHelper.SeedServiceOrderAsync(db, vehicleId,  serviceTypeId, mechanicId, pendingId);
        await SeedDataHelper.SeedServiceOrderAsync(db, vehicleId2, serviceTypeId, mechanicId, inProgressId);

        var result = await repo.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new ServiceOrderFilter { OrderStatusId = pendingId });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(o => o.OrderStatusId == pendingId);
    }
}

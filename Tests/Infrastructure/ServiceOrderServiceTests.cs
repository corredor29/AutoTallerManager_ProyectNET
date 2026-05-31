using Application.Common.Pagination;
using Application.Filters;
using Application.Requests.ServiceOrders;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class ServiceOrderServiceTests
{
    // ── helpers ─────────────────────────────────────────────────────

    private static ServiceOrderService CreateService(AutoTallerDbContext db) =>
        new(new ServiceOrderRepository(db), db);

    private static async Task<(int vehicleId, int serviceTypeId, int mechanicId, int pendingStatusId)>
        SeedPrerequisitesAsync(AutoTallerDbContext db)
    {
        var statuses        = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var serviceTypes    = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId)    = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId       = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var (_, mechanicId) = await SeedDataHelper.SeedMechanicAsync(db);

        return (vehicleId, serviceTypes.DiagnosticsId, mechanicId, statuses.PendingId);
    }

    // InMemory enforces NOT-NULL columns, so WorkPerformed and Notes must be non-null
    private static CreateServiceOrderRequest BuildRequest(
        int vehicleId, int serviceTypeId, int mechanicId, int orderStatusId,
        DateTime? estimatedDeliveryAt = null) =>
        new()
        {
            VehicleId           = vehicleId,
            ServiceTypeId       = serviceTypeId,
            MechanicId          = mechanicId,
            OrderStatusId       = orderStatusId,
            WorkPerformed       = "Routine inspection",
            Notes               = "No issues found",
            EstimatedDeliveryAt = estimatedDeliveryAt
        };

    // ── CreateAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_VehicleHasNoActiveOrder_CreatesServiceOrderSuccessfully()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);

        var result = await service.CreateAsync(
            BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.VehicleId.Should().Be(vehicleId);
        result.ServiceTypeId.Should().Be(serviceTypeId);
        result.MechanicId.Should().Be(mechanicId);
    }

    [Fact]
    public async Task CreateAsync_VehicleAlreadyHasActiveOrder_ThrowsInvalidOperationException()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);

        await service.CreateAsync(BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        var act = () => service.CreateAsync(BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*active service order*");
    }

    [Fact]
    public async Task CreateAsync_NoEstimatedDeliveryAt_AutoCalculatesFromServiceTypeDuration()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);
        var before  = DateTime.UtcNow;

        var result = await service.CreateAsync(
            BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));  // EstimatedDeliveryAt = null

        result.EstimatedDeliveryAt.Should().NotBeNull();
        result.EstimatedDeliveryAt!.Value.Should().BeAfter(before.AddHours(1));
        result.EstimatedDeliveryAt.Value.Should().BeBefore(before.AddHours(4));
    }

    [Fact]
    public async Task CreateAsync_EstimatedDeliveryAtProvided_UsesClientValue()
    {
        var db         = DbContextFactory.Create();
        var service    = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);
        var clientDate = DateTime.UtcNow.AddDays(3);

        var result = await service.CreateAsync(
            BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId, clientDate));

        result.EstimatedDeliveryAt.Should().BeCloseTo(clientDate, TimeSpan.FromSeconds(1));
    }

    // ── ChangeStatusAsync ────────────────────────────────────────────

    [Fact]
    public async Task ChangeStatusAsync_OrderExists_ChangesStatusSuccessfully()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);
        var completedId = (await db.OrderStatuses.ToListAsync())
            .First(s => s.Name.Value == "Completed").Id;

        var created = await service.CreateAsync(
            BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        var result = await service.ChangeStatusAsync(
            created.Id,
            new ChangeServiceOrderStatusRequest { OrderStatusId = completedId });

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ChangeStatusAsync_OrderNotFound_ReturnsFalse()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        await SeedDataHelper.SeedOrderStatusesAsync(db);
        var completedId = (await db.OrderStatuses.ToListAsync())
            .First(s => s.Name.Value == "Completed").Id;

        var result = await service.ChangeStatusAsync(
            9999,
            new ChangeServiceOrderStatusRequest { OrderStatusId = completedId });

        result.Should().BeFalse();
    }

    // ── GetAllPagedAsync ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllPagedAsync_NoFilters_ReturnsPaginatedResult()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);

        await service.CreateAsync(BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new ServiceOrderFilter());

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}

using Application.Common.Pagination;
using Application.Filters;
using Application.Requests.ServiceOrders;
using Domain.Entities.Appointments;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Appointments.Appointment;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.ServiceOrders.ServiceOrder;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class ServiceOrderServiceTests
{
    // ── helpers ─────────────────────────────────────────────────────

    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static ServiceOrderService CreateService(AutoTallerDbContext db)
    {
        // Mock del hub de SignalR — no necesita hacer nada real en los tests.
        var mockHub = new Mock<IHubContext<NotificationHub>>();
        mockHub
            .Setup(h => h.Clients.All.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new ServiceOrderService(new ServiceOrderRepository(db), db, mockHub.Object);
    }

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
    // Construye un objeto de prueba con valores validos por defecto.
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

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_NoEstimatedDeliveryAt_AutoCalculatesFromServiceTypeDuration()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);
        var before  = DateTime.UtcNow;

        var result = await service.CreateAsync(
            BuildRequest(vehicleId, serviceTypeId, mechanicId, pendingId));

        result.EstimatedDeliveryAt.Should().NotBeNull();
        result.EstimatedDeliveryAt!.Value.Should().BeAfter(before.AddHours(1));
        result.EstimatedDeliveryAt.Value.Should().BeBefore(before.AddHours(4));
    }

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_MechanicHasConflictingAppointment_ThrowsInvalidOperationException()
    {
        var db = DbContextFactory.Create();
        var service = CreateService(db);
        var (vehicleId, serviceTypeId, mechanicId, pendingId) = await SeedPrerequisitesAsync(db);
        var appointmentStatusId = await SeedDataHelper.SeedAppointmentStatusPendingAsync(db);
        var person = new Person(new PersonFirstName("Marta"), new PersonLastName("Diaz"));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var customer = new Customer(person.Id);
        await db.Customers.AddAsync(customer);
        await db.SaveChangesAsync();

        await db.Appointments.AddAsync(new Appointment(
            customer.Id,
            vehicleId,
            serviceTypeId,
            appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddMinutes(15)),
            new AppointmentNotes("Reserved slot"),
            mechanicId));
        await db.SaveChangesAsync();

        var act = () => service.CreateAsync(
            BuildRequest(
                vehicleId,
                serviceTypeId,
                mechanicId,
                pendingId,
                DateTime.UtcNow.AddHours(2)));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already has an appointment scheduled*");
    }

    // ── ChangeStatusAsync ────────────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
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

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task GetAllPagedAsync_FilterByCustomerId_ReturnsOnlyMatchingOrders()
    {
        var db                  = DbContextFactory.Create();
        var service             = CreateService(db);
        var statuses            = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var appointmentStatusId = await SeedDataHelper.SeedAppointmentStatusPendingAsync(db);
        var serviceTypes        = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId)        = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId           = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var (_, mechanicId)     = await SeedDataHelper.SeedMechanicAsync(db);

        var personA = new Person(new PersonFirstName("Paula"), new PersonLastName("Lopez"));
        var personB = new Person(new PersonFirstName("Diego"), new PersonLastName("Rios"));
        await db.Persons.AddRangeAsync(personA, personB);
        await db.SaveChangesAsync();

        var customerA = new Customer(personA.Id);
        var customerB = new Customer(personB.Id);
        await db.Customers.AddRangeAsync(customerA, customerB);
        await db.SaveChangesAsync();

        var appointmentA = new Appointment(
            customerA.Id,
            vehicleId,
            serviceTypes.DiagnosticsId,
            appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddDays(1)),
            new AppointmentNotes("Customer A"),
            mechanicId);

        var appointmentB = new Appointment(
            customerB.Id,
            vehicleId,
            serviceTypes.DiagnosticsId,
            appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddDays(2)),
            new AppointmentNotes("Customer B"),
            mechanicId);

        await db.Appointments.AddRangeAsync(appointmentA, appointmentB);
        await db.SaveChangesAsync();

        await db.ServiceOrders.AddRangeAsync(
            new ServiceOrder(
                vehicleId,
                serviceTypes.DiagnosticsId,
                mechanicId,
                statuses.PendingId,
                new WorkDescription("Order A"),
                new ServiceOrderNotes("Notes A"),
                appointmentA.Id,
                DateTime.UtcNow.AddDays(1).AddHours(2)),
            new ServiceOrder(
                vehicleId,
                serviceTypes.RepairId,
                mechanicId,
                statuses.PendingId,
                new WorkDescription("Order B"),
                new ServiceOrderNotes("Notes B"),
                appointmentB.Id,
                DateTime.UtcNow.AddDays(2).AddHours(4)));
        await db.SaveChangesAsync();

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new ServiceOrderFilter { CustomerId = customerA.Id });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        var order = result.Items.Single();
        order.CustomerId.Should().Be(customerA.Id);
        order.CustomerName.Should().Be("Paula Lopez");
    }
}
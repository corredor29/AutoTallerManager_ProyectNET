using Application.Common.Pagination;
using Application.Filters;
using Application.Requests.Invoices;
using Domain.Entities.Appointments;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Appointments.Appointment;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.ServiceOrders.ServiceOrder;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class InvoiceServiceTests
{
    private static InvoiceService CreateService(AutoTallerDbContext db) =>
        new(new InvoiceRepository(db), db, HubContextFactory.Create());

    [Fact]
    public async Task GetAllPagedAsync_FilterByCustomerId_ReturnsOnlyMatchingInvoices()
    {
        var db = DbContextFactory.Create();
        var service = CreateService(db);
        var statuses = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var appointmentStatusId = await SeedDataHelper.SeedAppointmentStatusPendingAsync(db);
        var serviceTypes = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId) = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleAId = await SeedDataHelper.SeedVehicleAsync(db, modelId, "1HGCM82633A004510");
        var vehicleBId = await SeedDataHelper.SeedVehicleAsync(db, modelId, "1HGCM82633A004511");
        var (_, mechanicId) = await SeedDataHelper.SeedMechanicAsync(db);

        var personA = new Person(new PersonFirstName("Laura"), new PersonLastName("Mendez"));
        var personB = new Person(new PersonFirstName("Tomas"), new PersonLastName("Vega"));
        await db.Persons.AddRangeAsync(personA, personB);
        await db.SaveChangesAsync();

        var customerA = new Customer(personA.Id);
        var customerB = new Customer(personB.Id);
        await db.Customers.AddRangeAsync(customerA, customerB);
        await db.SaveChangesAsync();

        var appointmentA = new Appointment(
            customerA.Id,
            vehicleAId,
            serviceTypes.DiagnosticsId,
            appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddDays(1)),
            new AppointmentNotes("Invoice A"),
            mechanicId);
        var appointmentB = new Appointment(
            customerB.Id,
            vehicleBId,
            serviceTypes.RepairId,
            appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddDays(2)),
            new AppointmentNotes("Invoice B"),
            mechanicId);
        await db.Appointments.AddRangeAsync(appointmentA, appointmentB);
        await db.SaveChangesAsync();

        var orderA = new ServiceOrder(
            vehicleAId,
            serviceTypes.DiagnosticsId,
            mechanicId,
            statuses.CompletedId,
            new WorkDescription("Work A"),
            new ServiceOrderNotes("Notes A"),
            appointmentA.Id,
            DateTime.UtcNow.AddHours(3));
        orderA.Close();

        var orderB = new ServiceOrder(
            vehicleBId,
            serviceTypes.RepairId,
            mechanicId,
            statuses.CompletedId,
            new WorkDescription("Work B"),
            new ServiceOrderNotes("Notes B"),
            appointmentB.Id,
            DateTime.UtcNow.AddHours(6));
        orderB.Close();

        await db.ServiceOrders.AddRangeAsync(orderA, orderB);
        await db.SaveChangesAsync();

        await service.CreateAsync(new CreateInvoiceRequest
        {
            ServiceOrderId = orderA.Id,
            LaborCost = 100m,
            Tax = 19m
        });

        await service.CreateAsync(new CreateInvoiceRequest
        {
            ServiceOrderId = orderB.Id,
            LaborCost = 200m,
            Tax = 38m
        });

        var result = await service.GetAllPagedAsync(
            new PaginationParams { PageNumber = 1, PageSize = 10 },
            new InvoiceFilter { CustomerId = customerA.Id });

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        var invoice = result.Items.Single();
        invoice.CustomerId.Should().Be(customerA.Id);
        invoice.CustomerName.Should().Be("Laura Mendez");
        invoice.VehicleVin.Should().Be("1HGCM82633A004510");
    }
}

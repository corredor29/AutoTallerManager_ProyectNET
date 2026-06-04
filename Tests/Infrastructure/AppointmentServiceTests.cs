using Application.Requests.Appointments;
using Domain.Entities.Appointments;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.ValueObject.Appointments.Appointment;
using Domain.ValueObject.Persons.Person;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class AppointmentServiceTests
{
    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static AppointmentService CreateService(AutoTallerDbContext db) =>
        new(new AppointmentRepository(db), db);

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_MechanicHasConflictingAppointment_ThrowsInvalidOperationException()
    {
        var db = DbContextFactory.Create();
        var service = CreateService(db);

        var appointmentStatusId = await SeedDataHelper.SeedAppointmentStatusPendingAsync(db);
        var serviceTypes = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId) = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var (_, mechanicId) = await SeedDataHelper.SeedMechanicAsync(db);
        var person = new Person(new PersonFirstName("Ana"), new PersonLastName("Torres"));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var customer = new Customer(person.Id);
        await db.Customers.AddAsync(customer);
        await db.SaveChangesAsync();

        var appointmentDate = DateTime.UtcNow.AddDays(1).AddHours(2);

        await db.Appointments.AddAsync(new Appointment(
            customer.Id,
            vehicleId,
            serviceTypes.DiagnosticsId,
            appointmentStatusId,
            new AppointmentDate(appointmentDate),
            new AppointmentNotes("Existing appointment"),
            mechanicId));
        await db.SaveChangesAsync();

        var act = () => service.CreateAsync(new CreateAppointmentRequest
        {
            CustomerId = customer.Id,
            VehicleId = vehicleId,
            ServiceTypeId = serviceTypes.DiagnosticsId,
            AppointmentStatusId = appointmentStatusId,
            AssignedUserId = mechanicId,
            AppointmentDate = appointmentDate.AddMinutes(30),
            Notes = "Overlapping appointment"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already has another appointment*");
    }
}

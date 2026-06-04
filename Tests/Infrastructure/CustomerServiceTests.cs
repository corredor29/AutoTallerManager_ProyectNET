using Application.Requests.Customers;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Mapster;
using Application.Mapping;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.PhoneCode;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class CustomerServiceTests
{
    static CustomerServiceTests()
    {
        MapsterConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    private static CustomerService CreateService(AutoTallerDbContext db) =>
        new(new CustomerRepository(db), db, HubContextFactory.Create());

    // ── CreateAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesCustomerWithPersonData()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var result = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "María",
            LastName  = "García"
        });

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Person.Should().NotBeNull();
        result.Person.FirstName.Should().Be("María");
        result.Person.LastName.Should().Be("García");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_MultipleCustomers_EachGetsUniqueId()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var c1 = await service.CreateAsync(new CreateCustomerRequest { FirstName = "Ana", LastName = "López" });
        var c2 = await service.CreateAsync(new CreateCustomerRequest { FirstName = "Luis", LastName = "Pérez" });

        c1.Id.Should().NotBe(c2.Id);
    }

    [Fact]
    public async Task RegisterWithVehicleAsync_ValidRequest_CreatesCustomerContactAndVehicle()
    {
        var db = DbContextFactory.Create();
        var service = CreateService(db);
        var (_, modelId) = await SeedDataHelper.SeedVehicleModelAsync(db);

        var phoneCode = new PhoneCode(new PhoneCodeValue("+57"), new PhoneCodeCountry("Colombia"));
        await db.PhoneCodes.AddAsync(phoneCode);
        await db.SaveChangesAsync();

        var result = await service.RegisterWithVehicleAsync(new RegisterCustomerWithVehicleRequest
        {
            FirstName = "Laura",
            LastName = "Ruiz",
            Email = "laura.ruiz@gmail.com",
            PhoneCodeId = phoneCode.Id,
            PhoneNumber = "3001234567",
            Vehicle = new RegisterVehicleRequest
            {
                ModelId = modelId,
                Vin = "1HGCM82633A004399",
                Year = 2022,
                Mileage = 12000,
                LicensePlate = "ABC123"
            }
        });

        result.Customer.Person.PrimaryEmail.Should().Be("laura.ruiz@gmail.com");
        result.Customer.Person.PrimaryPhone.Should().Be("3001234567");
        result.Vehicle.Vin.Should().Be("1HGCM82633A004399");

        db.VehicleOwnershipHistories.Should().ContainSingle(x => x.CustomerId == result.Customer.Id && x.VehicleId == result.Vehicle.Id);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_CustomerNotFound_ReturnsFalse()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var result = await service.DeleteAsync(9999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_CustomerHasAppointments_ThrowsInvalidOperationException()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        // Create the customer
        var customer = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Pedro",
            LastName  = "Ramírez"
        });

        // Seed prerequisites for Appointment
        var statuses         = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var serviceTypes     = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId)     = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId        = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var appointmentStatusId = await SeedDataHelper.SeedAppointmentStatusPendingAsync(db);

        await SeedDataHelper.SeedAppointmentAsync(
            db, customer.Id, vehicleId, serviceTypes.DiagnosticsId, appointmentStatusId);

        var act = () => service.DeleteAsync(customer.Id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*appointments*");
    }

    [Fact]
    public async Task DeleteAsync_CustomerWithNoDependencies_DeletesSuccessfully()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var customer = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Jorge",
            LastName  = "Montoya"
        });

        var result = await service.DeleteAsync(customer.Id);

        result.Should().BeTrue();

        var fetched = await service.GetByIdAsync(customer.Id);
        fetched.Should().BeNull();
    }
}

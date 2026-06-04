using Application.Requests.Vehicles;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AutoTallerManager.Tests.Infrastructure;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class VehicleServiceTests
{
    // Construye una instancia auxiliar para simplificar la preparacion del escenario.
    private static VehicleService CreateService(AutoTallerDbContext db)
    {
        // Mock del hub de SignalR — no necesita hacer nada real en los tests.
        var mockHub = new Mock<IHubContext<NotificationHub>>();
        mockHub
            .Setup(h => h.Clients.All.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new VehicleService(new VehicleRepository(db), db, mockHub.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesVehicleSuccessfully()
    {
        var db            = DbContextFactory.Create();
        var service       = CreateService(db);
        var (_, modelId)  = await SeedDataHelper.SeedVehicleModelAsync(db);

        var result = await service.CreateAsync(new CreateVehicleRequest
        {
            ModelId = modelId,
            Vin     = "1HGCM82633A004352",
            Year    = 2021,
            Mileage = 8_000
        });

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Vin.Should().Be("1HGCM82633A004352");
        result.Year.Should().Be(2021);
        result.Mileage.Should().Be(8_000);
        result.ModelName.Should().Be("Corolla");
        result.BrandName.Should().Be("Toyota");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_DuplicateVin_ThrowsInvalidOperationException()
    {
        var db           = DbContextFactory.Create();
        var service      = CreateService(db);
        var (_, modelId) = await SeedDataHelper.SeedVehicleModelAsync(db);

        await service.CreateAsync(new CreateVehicleRequest
        {
            ModelId = modelId, Vin = "1HGCM82633A004352", Year = 2021, Mileage = 0
        });

        var act = () => service.CreateAsync(new CreateVehicleRequest
        {
            ModelId = modelId, Vin = "1HGCM82633A004352", Year = 2022, Mileage = 0
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already registered*");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task CreateAsync_ModelDoesNotExist_ThrowsArgumentException()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var act = () => service.CreateAsync(new CreateVehicleRequest
        {
            ModelId = 9999, Vin = "1HGCM82633A004352", Year = 2021, Mileage = 0
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*model*");
    }

    // ── UpdateAsync (mileage) ────────────────────────────────────────

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task UpdateAsync_ValidMileage_UpdatesVehicleSuccessfully()
    {
        var db           = DbContextFactory.Create();
        var service      = CreateService(db);
        var (_, modelId) = await SeedDataHelper.SeedVehicleModelAsync(db);

        var created = await service.CreateAsync(new CreateVehicleRequest
        {
            ModelId = modelId, Vin = "1HGCM82633A004352", Year = 2021, Mileage = 10_000
        });

        var result = await service.UpdateAsync(created.Id, new UpdateVehicleRequest
        {
            Year    = 2021,
            Mileage = 25_000,
            ColorId = null, FuelTypeId = null, TransmissionTypeId = null
        });

        result.Should().BeTrue();

        var updated = await service.GetByIdAsync(created.Id);
        updated!.Mileage.Should().Be(25_000);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public async Task UpdateAsync_VehicleNotFound_ReturnsFalse()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);

        var result = await service.UpdateAsync(9999, new UpdateVehicleRequest
        {
            Year = 2020, Mileage = 5_000,
            ColorId = null, FuelTypeId = null, TransmissionTypeId = null
        });

        result.Should().BeFalse();
    }
}
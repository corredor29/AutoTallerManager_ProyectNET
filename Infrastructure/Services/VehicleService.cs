using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Vehicles;
using Application.Requests.Vehicles;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.Vehicle;
using Infrastructure.Context;

namespace Infrastructure.Services
{
    public sealed class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly AutoTallerDbContext _dbContext;

        public VehicleService(IVehicleRepository vehicleRepository, AutoTallerDbContext dbContext)
        {
            _vehicleRepository = vehicleRepository;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => MapToDto(v));
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle is null ? null : MapToDto(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
        {
            var vehicle = new Vehicle(
                request.ModelId,
                new VinNumber(request.Vin),
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate) ? null : new LicensePlate(request.LicensePlate));

            await _vehicleRepository.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.Model).LoadAsync();
            await _dbContext.Entry(vehicle.Model).Reference(m => m.Brand).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.Color).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.FuelType).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.TransmissionType).LoadAsync();

            return MapToDto(vehicle);
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleRequest request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle is null)
            {
                return false;
            }

            vehicle.Update(
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate) ? null : new LicensePlate(request.LicensePlate));

            _vehicleRepository.Update(vehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle is null)
            {
                return false;
            }

            _vehicleRepository.Remove(vehicle);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static VehicleDto MapToDto(Vehicle vehicle)
        {
            return new VehicleDto
            {
                Id = vehicle.Id,
                ModelId = vehicle.ModelId,
                ColorId = vehicle.ColorId,
                FuelTypeId = vehicle.FuelTypeId,
                TransmissionTypeId = vehicle.TransmissionTypeId,
                Vin = vehicle.VIN.Value,
                Year = vehicle.Year.Value,
                Mileage = vehicle.Mileage.Value,
                LicensePlate = vehicle.LicensePlate?.Value,
                ModelName = vehicle.Model?.ModelName.Value ?? string.Empty,
                BrandName = vehicle.Model?.Brand?.BrandName.Value ?? string.Empty,
                ColorName = vehicle.Color?.Name.Value,
                FuelTypeName = vehicle.FuelType?.Name.Value,
                TransmissionTypeName = vehicle.TransmissionType?.Name.Value,
            };
        }
    }
}

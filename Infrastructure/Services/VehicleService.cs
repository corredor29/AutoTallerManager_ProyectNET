using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Vehicles;
using Application.Requests.Vehicles;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.Vehicle;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedResult<VehicleDto>> GetAllAsync(GetVehiclesRequest request)
        {
            var query = _dbContext.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";
                query = query.Where(v =>
                    EF.Functions.ILike(v.VIN.Value, pattern) ||
                    (v.LicensePlate != null && EF.Functions.ILike(v.LicensePlate.Value, pattern)) ||
                    EF.Functions.ILike(v.Model.ModelName.Value, pattern) ||
                    EF.Functions.ILike(v.Model.Brand.BrandName.Value, pattern));
            }

            if (!string.IsNullOrWhiteSpace(request.Vin))
            {
                var pattern = $"%{request.Vin.Trim()}%";
                query = query.Where(v => EF.Functions.ILike(v.VIN.Value, pattern));
            }

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                var pattern = $"%{request.Brand.Trim()}%";
                query = query.Where(v => EF.Functions.ILike(v.Model.Brand.BrandName.Value, pattern));
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                var pattern = $"%{request.Model.Trim()}%";
                query = query.Where(v => EF.Functions.ILike(v.Model.ModelName.Value, pattern));
            }

            if (!string.IsNullOrWhiteSpace(request.LicensePlate))
            {
                var pattern = $"%{request.LicensePlate.Trim()}%";
                query = query.Where(v => v.LicensePlate != null && EF.Functions.ILike(v.LicensePlate.Value, pattern));
            }

            var totalCount = await query.CountAsync();
            var pageNumber = request.NormalizedPageNumber;
            var pageSize = request.NormalizedPageSize;

            var vehicles = await query
                .OrderBy(v => v.Model.Brand.BrandName.Value)
                .ThenBy(v => v.Model.ModelName.Value)
                .ThenBy(v => v.VIN.Value)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<VehicleDto>
            {
                Items = vehicles.Select(MapToDto).ToArray(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle is null ? null : MapToDto(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
        {
            if (!await _dbContext.VehicleModels.AnyAsync(model => model.Id == request.ModelId))
            {
                throw new ArgumentException($"Vehicle model {request.ModelId} does not exist.");
            }

            if (await _vehicleRepository.ExistsByVinAsync(request.Vin))
            {
                throw new InvalidOperationException($"Vehicle VIN '{request.Vin}' is already registered.");
            }

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

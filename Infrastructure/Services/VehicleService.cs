using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Notifications;
using Application.DTOs.Vehicles;
using Application.Filters;
using Application.Requests.Vehicles;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.Vehicle;
using Infrastructure.Context;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository          _vehicleRepository;
        private readonly AutoTallerDbContext          _dbContext;
        private readonly IHubContext<NotificationHub> _hub;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            AutoTallerDbContext dbContext,
            IHubContext<NotificationHub> hub)
        {
            _vehicleRepository = vehicleRepository;
            _dbContext         = dbContext;
            _hub               = hub;
        }

        public async Task<PagedResult<VehicleDto>> GetAllPagedAsync(
            PaginationParams pagination, VehicleFilter filter)
        {
            var result = await _vehicleRepository.GetAllPagedAsync(pagination, filter);
            return new PagedResult<VehicleDto>
            {
                Items      = result.Items.Select(MapToDto).ToArray(),
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle is null ? null : MapToDto(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
        {
            if (!await _dbContext.VehicleModels.AnyAsync(m => m.Id == request.ModelId))
                throw new ArgumentException($"Vehicle model {request.ModelId} does not exist.");

            if (await _vehicleRepository.ExistsByVinAsync(request.Vin))
                throw new InvalidOperationException($"Vehicle VIN '{request.Vin}' is already registered.");

            var vehicle = new Vehicle(
                request.ModelId,
                new VinNumber(request.Vin),
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate)
                    ? null
                    : new LicensePlate(request.LicensePlate));

            await _vehicleRepository.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();

            if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
            {
                var ownership = new VehicleOwnershipHistory(
                    vehicle.Id,
                    request.CustomerId.Value,
                    new Domain.ValueObject.Vehicles.VehicleOwnershipHistory.DateRange(
                        DateOnly.FromDateTime(DateTime.UtcNow), null));
                await _dbContext.VehicleOwnershipHistories.AddAsync(ownership);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.Entry(vehicle).Reference(v => v.Model).LoadAsync();
            await _dbContext.Entry(vehicle.Model).Reference(m => m.Brand).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.Color).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.FuelType).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.TransmissionType).LoadAsync();
            await _dbContext.Entry(vehicle).Collection(v => v.Ownerships).Query()
                .Include(o => o.Customer).ThenInclude(c => c.Person)
                .LoadAsync();

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "create",
                Entity     = "Vehicle",
                RecordId   = vehicle.Id,
                Message    = $"New vehicle VIN {request.Vin} registered",
                OccurredAt = DateTime.UtcNow
            });

            return MapToDto(vehicle);
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleRequest request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle is null) return false;

            vehicle.Update(
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate)
                    ? null
                    : new LicensePlate(request.LicensePlate));

            _vehicleRepository.Update(vehicle);
            await _dbContext.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "update",
                Entity     = "Vehicle",
                RecordId   = id,
                Message    = $"Vehicle #{id} updated",
                OccurredAt = DateTime.UtcNow
            });

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle is null) return false;

            await EnsureVehicleCanBeDeletedAsync(id);
            _vehicleRepository.Remove(vehicle);
            await _dbContext.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "delete",
                Entity     = "Vehicle",
                RecordId   = id,
                Message    = $"Vehicle #{id} deleted",
                OccurredAt = DateTime.UtcNow
            });

            return true;
        }

        private async Task EnsureVehicleCanBeDeletedAsync(int vehicleId)
        {
            var hasServiceOrders = await _dbContext.ServiceOrders
                .AnyAsync(x => x.VehicleId == vehicleId);
            if (hasServiceOrders)
                throw new InvalidOperationException(
                    $"Vehicle {vehicleId} cannot be deleted because it has service orders associated.");

            var hasAppointments = await _dbContext.Appointments
                .AnyAsync(x => x.VehicleId == vehicleId);
            if (hasAppointments)
                throw new InvalidOperationException(
                    $"Vehicle {vehicleId} cannot be deleted because it has appointments associated.");
        }

        private static VehicleDto MapToDto(Vehicle vehicle)
        {
            var currentOwner = vehicle.Ownerships?
                .FirstOrDefault(o => o.DateRange.EndDate == null);

            var ownerName = currentOwner?.Customer?.Person is not null
                ? $"{currentOwner.Customer.Person.FirstName.Value} {currentOwner.Customer.Person.LastName.Value}".Trim()
                : null;

            return new VehicleDto
            {
                Id                   = vehicle.Id,
                ModelId              = vehicle.ModelId,
                ColorId              = vehicle.ColorId,
                FuelTypeId           = vehicle.FuelTypeId,
                TransmissionTypeId   = vehicle.TransmissionTypeId,
                Vin                  = vehicle.VIN.Value,
                Year                 = vehicle.Year.Value,
                Mileage              = vehicle.Mileage.Value,
                LicensePlate         = vehicle.LicensePlate?.Value,
                ModelName            = vehicle.Model?.ModelName.Value        ?? string.Empty,
                BrandName            = vehicle.Model?.Brand?.BrandName.Value ?? string.Empty,
                ColorName            = vehicle.Color?.Name.Value,
                FuelTypeName         = vehicle.FuelType?.Name.Value,
                TransmissionTypeName = vehicle.TransmissionType?.Name.Value,
                CurrentOwnerName     = ownerName
            };
        }
    }
}
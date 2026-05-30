using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.VehicleOwnershipHistory;
using Application.Requests.VehicleOwnershipHistory;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.VehicleOwnershipHistory;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{

    public sealed class VehicleOwnershipHistoryService : IVehicleOwnershipHistoryService
    {
        private readonly IVehicleOwnershipHistoryRepository _ownershipRepository;
        private readonly AutoTallerDbContext                 _dbContext;

        public VehicleOwnershipHistoryService(IVehicleOwnershipHistoryRepository ownershipRepository,
                                            AutoTallerDbContext dbContext)
        {
            _ownershipRepository = ownershipRepository;
            _dbContext           = dbContext;
        }

        public async Task<IEnumerable<VehicleOwnershipHistoryDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var ownerships = await _ownershipRepository.GetByVehicleIdAsync(vehicleId);
            return ownerships.Select(MapToDto);
        }

        public async Task<IEnumerable<VehicleOwnershipHistoryDto>> GetByCustomerIdAsync(int customerId)
        {
            var ownerships = await _ownershipRepository.GetByCustomerIdAsync(customerId);
            return ownerships.Select(MapToDto);
        }

        public async Task<VehicleOwnershipHistoryDto?> GetCurrentOwnerAsync(int vehicleId)
        {
            var ownership = await _ownershipRepository.GetCurrentOwnerAsync(vehicleId);
            return ownership is null ? null : MapToDto(ownership);
        }

        public async Task<VehicleOwnershipHistoryDto> CreateAsync(CreateOwnershipRequest request)
        {
            if (await _ownershipRepository.HasActiveOwnershipAsync(request.VehicleId))
                throw new InvalidOperationException($"Vehicle {request.VehicleId} already has an active owner. Close the current ownership first.");

            var ownership = new VehicleOwnershipHistory(
                request.VehicleId,
                request.CustomerId,
                new DateRange(request.StartDate)
            );

            await _ownershipRepository.AddAsync(ownership);
            await _dbContext.SaveChangesAsync();

            return MapToDto(ownership);
        }

        public async Task<bool> CloseAsync(int id, CloseOwnershipRequest request)
        {
            var ownership = await _ownershipRepository.GetByIdAsync(id);
            if (ownership is null) return false;

            if (ownership.DateRange.EndDate is not null)
                throw new InvalidOperationException($"Ownership {id} is already closed.");

            ownership.Close(request.EndDate);

            _ownershipRepository.Update(ownership);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private static VehicleOwnershipHistoryDto MapToDto(VehicleOwnershipHistory ownership) => new()
        {
            Id         = ownership.Id,
            VehicleId  = ownership.VehicleId,
            CustomerId = ownership.CustomerId,
            StartDate  = ownership.DateRange.StartDate,
            EndDate    = ownership.DateRange.EndDate,
            FirstName  = ownership.Customer?.Person?.FirstName.Value ?? string.Empty,
            LastName   = ownership.Customer?.Person?.LastName.Value  ?? string.Empty,
            VIN        = ownership.Vehicle?.VIN.Value                ?? string.Empty
        };
    }
}
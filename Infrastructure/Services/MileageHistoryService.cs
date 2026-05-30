using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.MileageHistory;
using Application.Requests.MileageHistory;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.MileageHistory;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class MileageHistoryService : IMileageHistoryService
    {
        private readonly IMileageHistoryRepository _mileageHistoryRepository;
        private readonly AutoTallerDbContext        _dbContext;

        public MileageHistoryService(IMileageHistoryRepository mileageHistoryRepository,
                                    AutoTallerDbContext dbContext)
        {
            _mileageHistoryRepository = mileageHistoryRepository;
            _dbContext                = dbContext;
        }

        public async Task<IEnumerable<MileageHistoryDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var mileageHistories = await _mileageHistoryRepository.GetByVehicleIdAsync(vehicleId);
            return mileageHistories.Select(MapToDto);
        }

        public async Task<MileageHistoryDto?> GetLatestByVehicleIdAsync(int vehicleId)
        {
            var mileageHistory = await _mileageHistoryRepository.GetLatestByVehicleIdAsync(vehicleId);
            return mileageHistory is null ? null : MapToDto(mileageHistory);
        }

        public async Task<MileageHistoryDto?> GetByIdAsync(int id)
        {
            var mileageHistory = await _mileageHistoryRepository.GetByIdAsync(id);
            return mileageHistory is null ? null : MapToDto(mileageHistory);
        }

        public async Task<MileageHistoryDto> CreateAsync(CreateMileageHistoryRequest request)
        {
            var latest = await _mileageHistoryRepository.GetLatestByVehicleIdAsync(request.VehicleId);

            if (latest is not null && request.Mileage < latest.Mileage.Value)
                throw new InvalidOperationException($"New mileage {request.Mileage} cannot be less than current mileage {latest.Mileage.Value}.");

            var mileageHistory = new MileageHistory(
                request.VehicleId,
                new MileageRecord(request.Mileage),
                request.Notes
            );

            await _mileageHistoryRepository.AddAsync(mileageHistory);

            var vehicle = await _dbContext.Vehicles.FindAsync(request.VehicleId);
            if (vehicle is not null)
            {
                vehicle.UpdateMileage(new Domain.ValueObject.Vehicles.Vehicle.VehicleMileage(request.Mileage));
                _dbContext.Vehicles.Update(vehicle);
            }

            await _dbContext.SaveChangesAsync();

            return MapToDto(mileageHistory);
        }

        public async Task<bool> UpdateAsync(int id, UpdateMileageHistoryRequest request)
        {
            var mileageHistory = await _mileageHistoryRepository.GetByIdAsync(id);
            if (mileageHistory is null) return false;

            mileageHistory.UpdateNotes(request.Notes);

            _mileageHistoryRepository.Update(mileageHistory);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mileageHistory = await _mileageHistoryRepository.GetByIdAsync(id);
            if (mileageHistory is null) return false;

            _mileageHistoryRepository.Remove(mileageHistory);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private static MileageHistoryDto MapToDto(MileageHistory mileageHistory) => new()
        {
            Id         = mileageHistory.Id,
            VehicleId  = mileageHistory.VehicleId,
            Mileage    = mileageHistory.Mileage.Value,
            RecordedAt = mileageHistory.RecordedAt,
            Notes      = mileageHistory.Notes,
            VIN        = mileageHistory.Vehicle?.VIN.Value ?? string.Empty
        };
    }
}
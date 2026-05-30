using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.FuelTypes;
using Application.Requests.FuelTypes;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.FuelType;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class FuelTypeService : IFuelTypeService
    {
        private readonly IFuelTypeRepository _fuelTypeRepository;
        private readonly AutoTallerDbContext  _dbContext;

        public FuelTypeService(IFuelTypeRepository fuelTypeRepository,
                                AutoTallerDbContext dbContext)
        {
            _fuelTypeRepository = fuelTypeRepository;
            _dbContext          = dbContext;
        }

        public async Task<IEnumerable<FuelTypeDto>> GetAllAsync()
        {
            var fuelTypes = await _fuelTypeRepository.GetAllAsync();
            return fuelTypes.Select(MapToDto);
        }

        public async Task<FuelTypeDto?> GetByIdAsync(int id)
        {
            var fuelType = await _fuelTypeRepository.GetByIdAsync(id);
            return fuelType is null ? null : MapToDto(fuelType);
        }

        public async Task<FuelTypeDto> CreateAsync(CreateFuelTypeRequest request)
        {
            await EnsureNameIsUniqueAsync(request.Name);

            var fuelType = new FuelType(new FuelTypeName(request.Name));

            await _fuelTypeRepository.AddAsync(fuelType);
            await _dbContext.SaveChangesAsync();

            return MapToDto(fuelType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateFuelTypeRequest request)
        {
            var fuelType = await _fuelTypeRepository.GetByIdAsync(id);
            if (fuelType is null) return false;

            await EnsureNameIsUniqueAsync(request.Name, id);

            fuelType.Update(new FuelTypeName(request.Name));

            _fuelTypeRepository.Update(fuelType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fuelType = await _fuelTypeRepository.GetByIdAsync(id);
            if (fuelType is null) return false;

            if (await _dbContext.Vehicles.AnyAsync(x => x.FuelTypeId == id))
                throw new InvalidOperationException($"Fuel type {id} is being used and cannot be deleted.");

            _fuelTypeRepository.Remove(fuelType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.FuelTypes.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Fuel type '{name}' already exists.");
        }

        private static FuelTypeDto MapToDto(FuelType fuelType) => new()
        {
            Id   = fuelType.Id,
            Name = fuelType.Name.Value
        };
    }
}
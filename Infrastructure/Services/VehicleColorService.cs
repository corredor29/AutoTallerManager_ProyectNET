using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.VehicleColors;
using Application.Requests.VehicleColors;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.VehicleColor;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class VehicleColorService : IVehicleColorService
    {
        private readonly IVehicleColorRepository _vehicleColorRepository;
        private readonly AutoTallerDbContext      _dbContext;

        public VehicleColorService(IVehicleColorRepository vehicleColorRepository,
                                    AutoTallerDbContext dbContext)
        {
            _vehicleColorRepository = vehicleColorRepository;
            _dbContext              = dbContext;
        }

        public async Task<IEnumerable<VehicleColorDto>> GetAllAsync()
        {
            var colors = await _vehicleColorRepository.GetAllAsync();
            return colors.Select(MapToDto);
        }

        public async Task<VehicleColorDto?> GetByIdAsync(int id)
        {
            var color = await _vehicleColorRepository.GetByIdAsync(id);
            return color is null ? null : MapToDto(color);
        }

        public async Task<VehicleColorDto> CreateAsync(CreateVehicleColorRequest request)
        {
            await EnsureColorNameIsUniqueAsync(request.Name);

            var color = new VehicleColor(new ColorName(request.Name));

            await _vehicleColorRepository.AddAsync(color);
            await _dbContext.SaveChangesAsync();

            return MapToDto(color);
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleColorRequest request)
        {
            var color = await _vehicleColorRepository.GetByIdAsync(id);
            if (color is null) return false;

            await EnsureColorNameIsUniqueAsync(request.Name, id);

            color.Update(new ColorName(request.Name));

            _vehicleColorRepository.Update(color);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var color = await _vehicleColorRepository.GetByIdAsync(id);
            if (color is null) return false;

            if (await _dbContext.Vehicles.AnyAsync(x => x.ColorId == id))
                throw new InvalidOperationException($"Color {id} is being used and cannot be deleted.");

            _vehicleColorRepository.Remove(color);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureColorNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.VehicleColors.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Color '{name}' already exists.");
        }

        private static VehicleColorDto MapToDto(VehicleColor color) => new()
        {
            Id   = color.Id,
            Name = color.Name.Value
        };
    }
}
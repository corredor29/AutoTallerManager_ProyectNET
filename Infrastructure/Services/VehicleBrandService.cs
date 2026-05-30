using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.VehicleBrands;
using Application.Requests.VehicleBrands;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.VehicleBrand;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class VehicleBrandService : IVehicleBrandService
    {
        private readonly IVehicleBrandRepository _vehicleBrandRepository;
        private readonly AutoTallerDbContext      _dbContext;

        public VehicleBrandService(IVehicleBrandRepository vehicleBrandRepository,
                                    AutoTallerDbContext dbContext)
        {
            _vehicleBrandRepository = vehicleBrandRepository;
            _dbContext              = dbContext;
        }

        public async Task<IEnumerable<VehicleBrandDto>> GetAllAsync()
        {
            var brands = await _vehicleBrandRepository.GetAllAsync();
            return brands.Select(MapToDto);
        }

        public async Task<VehicleBrandDto?> GetByIdAsync(int id)
        {
            var brand = await _vehicleBrandRepository.GetByIdAsync(id);
            return brand is null ? null : MapToDto(brand);
        }

        public async Task<VehicleBrandDto> CreateAsync(CreateVehicleBrandRequest request)
        {
            await EnsureBrandNameIsUniqueAsync(request.BrandName);

            var brand = new VehicleBrand(new BrandName(request.BrandName));

            await _vehicleBrandRepository.AddAsync(brand);
            await _dbContext.SaveChangesAsync();

            return MapToDto(brand);
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleBrandRequest request)
        {
            var brand = await _vehicleBrandRepository.GetByIdAsync(id);
            if (brand is null) return false;

            await EnsureBrandNameIsUniqueAsync(request.BrandName, id);

            brand.Update(new BrandName(request.BrandName));

            _vehicleBrandRepository.Update(brand);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _vehicleBrandRepository.GetByIdAsync(id);
            if (brand is null) return false;

            if (await _dbContext.VehicleModels.AnyAsync(x => x.BrandId == id))
                throw new InvalidOperationException($"Brand {id} has models and cannot be deleted.");

            _vehicleBrandRepository.Remove(brand);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureBrandNameIsUniqueAsync(string brandName, int? excludeId = null)
        {
            var normalizedName = brandName.Trim().ToLower();
            var exists = await _dbContext.VehicleBrands.AnyAsync(x =>
                x.BrandName.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Brand '{brandName}' already exists.");
        }

        private static VehicleBrandDto MapToDto(VehicleBrand brand) => new()
        {
            Id        = brand.Id,
            BrandName = brand.BrandName.Value
        };
    }
}
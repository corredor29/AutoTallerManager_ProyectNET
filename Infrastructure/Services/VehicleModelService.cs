using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.VehicleModels;
using Application.Requests.VehicleModels;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.VehicleModel;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class VehicleModelService : IVehicleModelService
    {
        private readonly IVehicleModelRepository _vehicleModelRepository;
        private readonly AutoTallerDbContext      _dbContext;

        public VehicleModelService(IVehicleModelRepository vehicleModelRepository,
                                    AutoTallerDbContext dbContext)
        {
            _vehicleModelRepository = vehicleModelRepository;
            _dbContext              = dbContext;
        }

        public async Task<IEnumerable<VehicleModelDto>> GetAllAsync()
        {
            var models = await _vehicleModelRepository.GetAllAsync();
            return models.Select(MapToDto);
        }

        public async Task<IEnumerable<VehicleModelDto>> GetByBrandIdAsync(int brandId)
        {
            var models = await _vehicleModelRepository.GetByBrandIdAsync(brandId);
            return models.Select(MapToDto);
        }

        public async Task<VehicleModelDto?> GetByIdAsync(int id)
        {
            var model = await _vehicleModelRepository.GetByIdAsync(id);
            return model is null ? null : MapToDto(model);
        }

        public async Task<VehicleModelDto> CreateAsync(CreateVehicleModelRequest request)
        {
            await EnsureModelNameIsUniqueAsync(request.BrandId, request.ModelName);

            var model = new VehicleModel(
                request.BrandId,
                new ModelName(request.ModelName)
            );

            await _vehicleModelRepository.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            return MapToDto(model);
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleModelRequest request)
        {
            var model = await _vehicleModelRepository.GetByIdAsync(id);
            if (model is null) return false;

            await EnsureModelNameIsUniqueAsync(request.BrandId, request.ModelName, id);

            model.Update(new ModelName(request.ModelName));

            _vehicleModelRepository.Update(model);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _vehicleModelRepository.GetByIdAsync(id);
            if (model is null) return false;

            if (await _dbContext.Vehicles.AnyAsync(x => x.ModelId == id))
                throw new InvalidOperationException($"Model {id} has vehicles and cannot be deleted.");

            _vehicleModelRepository.Remove(model);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureModelNameIsUniqueAsync(int brandId, string modelName, int? excludeId = null)
        {
            var normalizedName = modelName.Trim().ToLower();
            var exists = await _dbContext.VehicleModels.AnyAsync(x =>
                x.BrandId == brandId &&
                x.ModelName.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Model '{modelName}' already exists for this brand.");
        }

        private static VehicleModelDto MapToDto(VehicleModel model) => new()
        {
            Id        = model.Id,
            BrandId   = model.BrandId,
            ModelName = model.ModelName.Value,
            BrandName = model.Brand?.BrandName.Value ?? string.Empty
        };
    }
}
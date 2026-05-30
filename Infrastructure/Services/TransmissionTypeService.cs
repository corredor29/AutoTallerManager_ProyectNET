using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.TransmissionTypes;
using Application.Requests.TransmissionTypes;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Vehicles.TransmissionType;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class TransmissionTypeService : ITransmissionTypeService
    {
        private readonly ITransmissionTypeRepository _transmissionTypeRepository;
        private readonly AutoTallerDbContext          _dbContext;

        public TransmissionTypeService(ITransmissionTypeRepository transmissionTypeRepository,
                                        AutoTallerDbContext dbContext)
        {
            _transmissionTypeRepository = transmissionTypeRepository;
            _dbContext                  = dbContext;
        }

        public async Task<IEnumerable<TransmissionTypeDto>> GetAllAsync()
        {
            var transmissionTypes = await _transmissionTypeRepository.GetAllAsync();
            return transmissionTypes.Select(MapToDto);
        }

        public async Task<TransmissionTypeDto?> GetByIdAsync(int id)
        {
            var transmissionType = await _transmissionTypeRepository.GetByIdAsync(id);
            return transmissionType is null ? null : MapToDto(transmissionType);
        }

        public async Task<TransmissionTypeDto> CreateAsync(CreateTransmissionTypeRequest request)
        {
            await EnsureNameIsUniqueAsync(request.Name);

            var transmissionType = new TransmissionType(
                new TransmissionTypeName(request.Name)
            );

            await _transmissionTypeRepository.AddAsync(transmissionType);
            await _dbContext.SaveChangesAsync();

            return MapToDto(transmissionType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTransmissionTypeRequest request)
        {
            var transmissionType = await _transmissionTypeRepository.GetByIdAsync(id);
            if (transmissionType is null) return false;

            await EnsureNameIsUniqueAsync(request.Name, id);

            transmissionType.Update(new TransmissionTypeName(request.Name));

            _transmissionTypeRepository.Update(transmissionType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transmissionType = await _transmissionTypeRepository.GetByIdAsync(id);
            if (transmissionType is null) return false;

            if (await _dbContext.Vehicles.AnyAsync(x => x.TransmissionTypeId == id))
                throw new InvalidOperationException($"Transmission type {id} is being used and cannot be deleted.");

            _transmissionTypeRepository.Remove(transmissionType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.TransmissionTypes.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Transmission type '{name}' already exists.");
        }

        private static TransmissionTypeDto MapToDto(TransmissionType transmissionType) => new()
        {
            Id   = transmissionType.Id,
            Name = transmissionType.Name.Value
        };
    }
}
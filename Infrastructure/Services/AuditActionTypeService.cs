using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.AuditActionTypes;
using Application.Requests.AuditActionTypes;
using Domain.Entities.Audit;
using Domain.ValueObject.Audit.AuditActionType;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Services
{
    public sealed class AuditActionTypeService : IAuditActionTypeService
    {
        private readonly IAuditActionTypeRepository _auditActionTypeRepository;
        private readonly AutoTallerDbContext         _dbContext;

        public AuditActionTypeService(IAuditActionTypeRepository auditActionTypeRepository,
                                    AutoTallerDbContext dbContext)
        {
            _auditActionTypeRepository = auditActionTypeRepository;
            _dbContext                 = dbContext;
        }

        public async Task<IEnumerable<AuditActionTypeDto>> GetAllAsync()
        {
            var actionTypes = await _auditActionTypeRepository.GetAllAsync();
            return actionTypes.Select(MapToDto);
        }

        public async Task<AuditActionTypeDto?> GetByIdAsync(int id)
        {
            var actionType = await _auditActionTypeRepository.GetByIdAsync(id);
            return actionType is null ? null : MapToDto(actionType);
        }

        public async Task<AuditActionTypeDto> CreateAsync(CreateAuditActionTypeRequest request)
        {
            await EnsureNameIsUniqueAsync(request.Name);

            var actionType = new AuditActionType(
                new AuditActionTypeName(request.Name)
            );

            await _auditActionTypeRepository.AddAsync(actionType);
            await _dbContext.SaveChangesAsync();

            return MapToDto(actionType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAuditActionTypeRequest request)
        {
            var actionType = await _auditActionTypeRepository.GetByIdAsync(id);
            if (actionType is null) return false;

            await EnsureNameIsUniqueAsync(request.Name, id);

            actionType.Update(new AuditActionTypeName(request.Name));

            _auditActionTypeRepository.Update(actionType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var actionType = await _auditActionTypeRepository.GetByIdAsync(id);
            if (actionType is null) return false;

            if (await _dbContext.AuditLogs.AnyAsync(x => x.AuditActionTypeId == id))
                throw new InvalidOperationException($"Audit action type {id} is being used and cannot be deleted.");

            _auditActionTypeRepository.Remove(actionType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();
            var exists = await _dbContext.AuditActionTypes.AnyAsync(x =>
                x.Name.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Audit action type '{name}' already exists.");
        }

        private static AuditActionTypeDto MapToDto(AuditActionType actionType) => new()
        {
            Id   = actionType.Id,
            Name = actionType.Name.Value
        };
    }
}
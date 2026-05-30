using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.AuditLogs;
using Application.Requests.AuditLogs;
using Domain.Entities.Audit;
using Domain.ValueObject.Audit.AuditLog;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly AutoTallerDbContext  _dbContext;

        public AuditLogService(IAuditLogRepository auditLogRepository,
                                AutoTallerDbContext dbContext)
        {
            _auditLogRepository = auditLogRepository;
            _dbContext          = dbContext;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _auditLogRepository.GetAllAsync();
            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId)
        {
            var logs = await _auditLogRepository.GetByUserIdAsync(userId);
            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string entityName)
        {
            var logs = await _auditLogRepository.GetByEntityAsync(entityName);
            return logs.Select(MapToDto);
        }

        public async Task<IEnumerable<AuditLogDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            if (from > to)
                throw new InvalidOperationException("From date cannot be greater than to date.");

            var logs = await _auditLogRepository.GetByDateRangeAsync(from, to);
            return logs.Select(MapToDto);
        }

        public async Task<AuditLogDto?> GetByIdAsync(int id)
        {
            var log = await _auditLogRepository.GetByIdAsync(id);
            return log is null ? null : MapToDto(log);
        }

        public async Task<AuditLogDto> CreateAsync(CreateAuditLogRequest request)
        {
            var log = new AuditLog(
                request.UserId,
                request.AuditActionTypeId,
                new AffectedEntityName(request.AffectedEntity),
                request.AffectedRecordId,
                new AuditDescription(request.Description)
            );

            await _auditLogRepository.AddAsync(log);
            await _dbContext.SaveChangesAsync();

            return MapToDto(log);
        }

        private static AuditLogDto MapToDto(AuditLog log) => new()
        {
            Id                = log.Id,
            UserId            = log.UserId,
            AuditActionTypeId = log.AuditActionTypeId,
            AffectedEntity    = log.AffectedEntity.Value,
            AffectedRecordId  = log.AffectedRecordId,
            OccurredAt        = log.OccurredAt,
            Description       = log.Description.Value,
            ActionName        = log.AuditActionType?.Name.Value    ?? string.Empty,
            UserFullName      = $"{log.User?.Person?.FirstName.Value ?? string.Empty} {log.User?.Person?.LastName.Value ?? string.Empty}".Trim()
        };
    }
}
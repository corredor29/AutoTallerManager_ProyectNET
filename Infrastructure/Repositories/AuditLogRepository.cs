using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Audit;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class AuditLogRepository : IAuditLogRepository
    {
        private readonly AutoTallerDbContext _context;

        public AuditLogRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
            => await _context.AuditLogs
                            .Include(x => x.User)
                                .ThenInclude(x => x.Person)
                            .Include(x => x.AuditActionType)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
            => await _context.AuditLogs
                            .Include(x => x.User)
                                .ThenInclude(x => x.Person)
                            .Include(x => x.AuditActionType)
                            .OrderByDescending(x => x.OccurredAt)
                            .ToListAsync();

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
            => await _context.AuditLogs
                            .Include(x => x.AuditActionType)
                            .Where(x => x.UserId == userId)
                            .OrderByDescending(x => x.OccurredAt)
                            .ToListAsync();

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName)
            => await _context.AuditLogs
                            .Include(x => x.User)
                                .ThenInclude(x => x.Person)
                            .Include(x => x.AuditActionType)
                            .Where(x => x.AffectedEntity.Value == entityName)
                            .OrderByDescending(x => x.OccurredAt)
                            .ToListAsync();

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await _context.AuditLogs
                            .Include(x => x.User)
                                .ThenInclude(x => x.Person)
                            .Include(x => x.AuditActionType)
                            .Where(x => x.OccurredAt >= from && x.OccurredAt <= to)
                            .OrderByDescending(x => x.OccurredAt)
                            .ToListAsync();

        public async Task AddAsync(AuditLog auditLog)
            => await _context.AuditLogs.AddAsync(auditLog);
    }
}
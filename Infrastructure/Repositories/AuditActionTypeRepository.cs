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
    public sealed class AuditActionTypeRepository : IAuditActionTypeRepository
    {
        private readonly AutoTallerDbContext _context;

        public AuditActionTypeRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<AuditActionType?> GetByIdAsync(int id)
            => await _context.AuditActionTypes
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<AuditActionType>> GetAllAsync()
            => await _context.AuditActionTypes
                            .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.AuditActionTypes
                            .AnyAsync(x => x.Name.Value == name);

        public async Task AddAsync(AuditActionType auditActionType)
            => await _context.AuditActionTypes.AddAsync(auditActionType);

        public void Update(AuditActionType auditActionType)
            => _context.AuditActionTypes.Update(auditActionType);

        public void Remove(AuditActionType auditActionType)
            => _context.AuditActionTypes.Remove(auditActionType);
    }
}
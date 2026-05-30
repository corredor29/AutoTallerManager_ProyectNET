using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Audit;

namespace Application.Contracts.Repositories
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?>            GetByIdAsync(int id);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task                        AddAsync(AuditLog auditLog);
    }
}
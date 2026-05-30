using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Audit;

namespace Application.Contracts.Repositories
{
    public interface IAuditActionTypeRepository
    {
        Task<AuditActionType?>            GetByIdAsync(int id);
        Task<IEnumerable<AuditActionType>> GetAllAsync();
        Task<bool>                        ExistsByNameAsync(string name);
        Task                              AddAsync(AuditActionType auditActionType);
        void                              Update(AuditActionType auditActionType);
        void                              Remove(AuditActionType auditActionType);
    }
}
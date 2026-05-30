using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AuditActionTypes;
using Application.Requests.AuditActionTypes;

namespace Application.Contracts.Services
{
    public interface IAuditActionTypeService
    {
        Task<IEnumerable<AuditActionTypeDto>> GetAllAsync();
        Task<AuditActionTypeDto?>             GetByIdAsync(int id);
        Task<AuditActionTypeDto>              CreateAsync(CreateAuditActionTypeRequest request);
        Task<bool>                            UpdateAsync(int id, UpdateAuditActionTypeRequest request);
        Task<bool>                            DeleteAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AuditLogs;
using Application.Requests.AuditLogs;

namespace Application.Contracts.Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string entityName);
        Task<IEnumerable<AuditLogDto>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<AuditLogDto?>             GetByIdAsync(int id);
        Task<AuditLogDto>              CreateAsync(CreateAuditLogRequest request);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.EmailDomains;
using Application.Requests.EmailDomains;

namespace Application.Contracts.Services
{

public interface IEmailDomainService
    {
        Task<IEnumerable<EmailDomainDto>> GetAllAsync();
        Task<EmailDomainDto?> GetByIdAsync(int id);
        Task<EmailDomainDto> CreateAsync(CreateEmailDomainRequest request);
        Task<bool> UpdateAsync(int id, UpdateEmailDomainRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
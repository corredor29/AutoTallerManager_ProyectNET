using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.EmailDomains;
using Application.Requests.EmailDomains;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.EmailDomain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{

    public sealed class EmailDomainService : IEmailDomainService
    {
        private readonly IEmailDomainRepository _emailDomainRepository;
        private readonly AutoTallerDbContext    _dbContext;

        public EmailDomainService(IEmailDomainRepository emailDomainRepository,
                                AutoTallerDbContext dbContext)
        {
            _emailDomainRepository = emailDomainRepository;
            _dbContext             = dbContext;
        }

        public async Task<IEnumerable<EmailDomainDto>> GetAllAsync()
        {
            var domains = await _emailDomainRepository.GetAllAsync();
            return domains.Select(MapToDto);
        }

        public async Task<EmailDomainDto?> GetByIdAsync(int id)
        {
            var domain = await _emailDomainRepository.GetByIdAsync(id);
            return domain is null ? null : MapToDto(domain);
        }

        public async Task<EmailDomainDto> CreateAsync(CreateEmailDomainRequest request)
        {
            await EnsureDomainIsUniqueAsync(request.Domain);

            var domain = new EmailDomain(new EmailDomainValue(request.Domain));
            await _emailDomainRepository.AddAsync(domain);
            await _dbContext.SaveChangesAsync();

            return MapToDto(domain);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEmailDomainRequest request)
        {
            var domain = await _emailDomainRepository.GetByIdAsync(id);
            if (domain is null) return false;

            await EnsureDomainIsUniqueAsync(request.Domain, id);

            domain.Update(new EmailDomainValue(request.Domain));
            _emailDomainRepository.Update(domain);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var domain = await _emailDomainRepository.GetByIdAsync(id);
            if (domain is null) return false;

            if (await _dbContext.PersonEmails.AnyAsync(x => x.EmailDomainId == id))
                throw new InvalidOperationException($"Email domain {id} is being used and cannot be deleted.");

            _emailDomainRepository.Remove(domain);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureDomainIsUniqueAsync(string domain, int? excludeId = null)
        {
            var normalizedDomain = domain.Trim().ToLower();
            var exists = await _dbContext.EmailDomains.AnyAsync(x =>
                x.Domain.Value.ToLower() == normalizedDomain &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Email domain '{domain}' already exists.");
        }

        private static EmailDomainDto MapToDto(EmailDomain domain) => new()
        {
            Id     = domain.Id,
            Domain = domain.Domain.Value
        };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Persons;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public sealed class EmailDomainRepository : IEmailDomainRepository
    {
        private readonly AutoTallerDbContext _context;

        public EmailDomainRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<EmailDomain?> GetByIdAsync(int id)
            => await _context.EmailDomains.FindAsync(id);

        public async Task<IEnumerable<EmailDomain>> GetAllAsync()
            => await _context.EmailDomains.ToListAsync();

        public async Task<bool> ExistsByDomainAsync(string domain)
            => await _context.EmailDomains.AnyAsync(x => x.Domain.Value == domain);

        public async Task AddAsync(EmailDomain emailDomain)
            => await _context.EmailDomains.AddAsync(emailDomain);

        public void Update(EmailDomain emailDomain)
            => _context.EmailDomains.Update(emailDomain);

        public void Remove(EmailDomain emailDomain)
            => _context.EmailDomains.Remove(emailDomain);
    }
}
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
    public sealed class PersonEmailRepository : IPersonEmailRepository
    {
        private readonly AutoTallerDbContext _context;

        public PersonEmailRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<PersonEmail?> GetByIdAsync(int id)
            => await _context.PersonEmails
                            .Include(x => x.Person)
                            .Include(x => x.EmailDomain)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<PersonEmail>> GetByPersonIdAsync(int personId)
            => await _context.PersonEmails
                            .Include(x => x.EmailDomain)
                            .Where(x => x.PersonId == personId)
                            .ToListAsync();

        public async Task<bool> ExistsByEmailAsync(string emailUser, int emailDomainId)
            => await _context.PersonEmails
                            .AnyAsync(x => x.EmailUser.Value == emailUser
                                        && x.EmailDomainId == emailDomainId);

        public async Task<PersonEmail?> GetPrimaryByPersonIdAsync(int personId)
            => await _context.PersonEmails
                            .Include(x => x.EmailDomain)
                            .FirstOrDefaultAsync(x => x.PersonId == personId
                                                    && x.IsPrimary == true);

        public async Task AddAsync(PersonEmail personEmail)
            => await _context.PersonEmails.AddAsync(personEmail);

        public void Update(PersonEmail personEmail)
            => _context.PersonEmails.Update(personEmail);

        public void Remove(PersonEmail personEmail)
            => _context.PersonEmails.Remove(personEmail);
    }
}
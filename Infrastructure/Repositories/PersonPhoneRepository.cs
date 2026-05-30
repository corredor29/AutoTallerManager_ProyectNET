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
    public sealed class PersonPhoneRepository : IPersonPhoneRepository
    {
        private readonly AutoTallerDbContext _context;

        public PersonPhoneRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<PersonPhone?> GetByIdAsync(int id)
            => await _context.PersonPhones
                            .Include(x => x.Person)
                            .Include(x => x.PhoneCode)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<PersonPhone>> GetByPersonIdAsync(int personId)
            => await _context.PersonPhones
                            .Include(x => x.PhoneCode)
                            .Where(x => x.PersonId == personId)
                            .ToListAsync();

        public async Task<bool> ExistsByPhoneAsync(int phoneCodeId, string phoneNumber)
            => await _context.PersonPhones
                            .AnyAsync(x => x.PhoneCodeId == phoneCodeId
                                        && x.PhoneNumber.Value == phoneNumber);

        public async Task<PersonPhone?> GetPrimaryByPersonIdAsync(int personId)
            => await _context.PersonPhones
                            .Include(x => x.PhoneCode)
                            .FirstOrDefaultAsync(x => x.PersonId == personId
                                                    && x.IsPrimary == true);

        public async Task AddAsync(PersonPhone personPhone)
            => await _context.PersonPhones.AddAsync(personPhone);

        public void Update(PersonPhone personPhone)
            => _context.PersonPhones.Update(personPhone);

        public void Remove(PersonPhone personPhone)
            => _context.PersonPhones.Remove(personPhone);
    }
}
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
    public sealed class PersonRepository : IPersonRepository
    {
        private readonly AutoTallerDbContext _context;

        public PersonRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetByIdAsync(int id)
            => await _context.Persons
                            .Include(x => x.Documents)
                            .Include(x => x.Emails)
                            .Include(x => x.Phones)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Person>> GetAllAsync()
            => await _context.Persons
                            .Include(x => x.Documents)
                            .Include(x => x.Emails)
                            .Include(x => x.Phones)
                            .ToListAsync();

        public async Task<bool> ExistsAsync(int id)
            => await _context.Persons.AnyAsync(x => x.Id == id);

        public async Task AddAsync(Person person)
            => await _context.Persons.AddAsync(person);

        public void Update(Person person)
            => _context.Persons.Update(person);

        public void Remove(Person person)
            => _context.Persons.Remove(person);
    }
}
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
    public sealed class PersonDocumentRepository : IPersonDocumentRepository
    {
        private readonly AutoTallerDbContext _context;

        public PersonDocumentRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<PersonDocument?> GetByIdAsync(int id)
            => await _context.PersonDocuments
                            .Include(x => x.Person)
                            .Include(x => x.DocumentType)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<PersonDocument>> GetByPersonIdAsync(int personId)
            => await _context.PersonDocuments
                            .Include(x => x.DocumentType)
                            .Where(x => x.PersonId == personId)
                            .ToListAsync();

        public async Task<bool> ExistsByDocumentNumberAsync(int documentTypeId, string documentNumber)
            => await _context.PersonDocuments
                            .AnyAsync(x => x.DocumentTypeId == documentTypeId
                                        && x.DocumentNumber.Value == documentNumber);

        public async Task<PersonDocument?> GetPrimaryByPersonIdAsync(int personId)
            => await _context.PersonDocuments
                            .Include(x => x.DocumentType)
                            .FirstOrDefaultAsync(x => x.PersonId == personId
                                                    && x.IsPrimary == true);

        public async Task AddAsync(PersonDocument personDocument)
            => await _context.PersonDocuments.AddAsync(personDocument);

        public void Update(PersonDocument personDocument)
            => _context.PersonDocuments.Update(personDocument);

        public void Remove(PersonDocument personDocument)
            => _context.PersonDocuments.Remove(personDocument);
    }
}
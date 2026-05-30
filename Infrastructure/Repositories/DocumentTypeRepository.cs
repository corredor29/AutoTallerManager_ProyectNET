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
    public sealed class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly AutoTallerDbContext _context;

        public DocumentTypeRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentType?> GetByIdAsync(int id)
            => await _context.DocumentTypes.FindAsync(id);

        public async Task<IEnumerable<DocumentType>> GetAllAsync()
            => await _context.DocumentTypes.ToListAsync();

        public async Task<bool> ExistsByCodeAsync(string code)
            => await _context.DocumentTypes.AnyAsync(x => x.Code.Value == code);

        public async Task AddAsync(DocumentType documentType)
            => await _context.DocumentTypes.AddAsync(documentType);

        public void Update(DocumentType documentType)
            => _context.DocumentTypes.Update(documentType);

        public void Remove(DocumentType documentType)
            => _context.DocumentTypes.Remove(documentType);
    }
}
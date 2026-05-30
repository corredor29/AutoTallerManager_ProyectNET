using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
    public interface IDocumentTypeRepository
    {
        Task<DocumentType?> GetByIdAsync(int id);
        Task<IEnumerable<DocumentType>> GetAllAsync();
        Task<bool> ExistsByCodeAsync(string code);
        Task AddAsync(DocumentType documentType);
        void Update(DocumentType documentType);
        void Remove(DocumentType documentType);
    }
}
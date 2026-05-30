using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
using Domain.Entities.Persons;

    public interface IPersonDocumentRepository
    {
        Task<PersonDocument?> GetByIdAsync(int id);
        Task<IEnumerable<PersonDocument>> GetByPersonIdAsync(int personId);
        Task<bool> ExistsByDocumentNumberAsync(int documentTypeId, string documentNumber);
        Task<PersonDocument?> GetPrimaryByPersonIdAsync(int personId);
        Task AddAsync(PersonDocument personDocument);
        void Update(PersonDocument personDocument);
        void Remove(PersonDocument personDocument);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
    public interface IPersonPhoneRepository
    {
        Task<PersonPhone?>            GetByIdAsync(int id);
        Task<IEnumerable<PersonPhone>> GetByPersonIdAsync(int personId);
        Task<bool>                    ExistsByPhoneAsync(int phoneCodeId, string phoneNumber);
        Task<PersonPhone?>            GetPrimaryByPersonIdAsync(int personId);
        Task                          AddAsync(PersonPhone personPhone);
        void                          Update(PersonPhone personPhone);
        void                          Remove(PersonPhone personPhone);
    }
}
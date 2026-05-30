using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
    public interface IPersonEmailRepository
    {
        Task<PersonEmail?> GetByIdAsync(int id);
        Task<IEnumerable<PersonEmail>> GetByPersonIdAsync(int personId);
        Task<bool> ExistsByEmailAsync(string emailUser, int emailDomainId);
        Task<PersonEmail?>            GetPrimaryByPersonIdAsync(int personId);
        Task AddAsync(PersonEmail personEmail);
        void Update(PersonEmail personEmail);
        void Remove(PersonEmail personEmail);
    }
}
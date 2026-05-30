using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
    public interface IPhoneCodeRepository
    {
        Task<PhoneCode?> GetByIdAsync(int id);
        Task<IEnumerable<PhoneCode>> GetAllAsync();
        Task<bool> ExistsByCodeAsync(string code);
        Task AddAsync(PhoneCode phoneCode);
        void Update(PhoneCode phoneCode);
        void Remove(PhoneCode phoneCode);
    }
}
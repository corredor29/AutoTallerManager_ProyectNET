using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
    public interface IPersonRepository
    {
        Task<Person?> GetByIdAsync(int id);
        Task<IEnumerable<Person>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Person person);
        void Update(Person person);
        void Remove(Person person);
    }
}
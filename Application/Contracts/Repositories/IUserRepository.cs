using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?>            GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?>            GetByPersonIdAsync(int personId);
        Task<bool>             ExistsByPersonIdAsync(int personId);
        Task                   AddAsync(User user);
        void                   Update(User user);
        void                   Remove(User user);
    }
}
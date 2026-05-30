using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Contracts.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?>            GetByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<bool>             ExistsByNameAsync(string roleName);
        Task                   AddAsync(Role role);
        void                   Update(Role role);
        void                   Remove(Role role);
    }
}
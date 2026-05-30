using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Contracts.Repositories
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
        Task<bool>                  ExistsAsync(int userId, int roleId);
        Task                        AddAsync(UserRole userRole);
        void                        Remove(UserRole userRole);
    }
}
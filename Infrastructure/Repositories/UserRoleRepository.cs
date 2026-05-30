using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Users;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class UserRoleRepository : IUserRoleRepository
    {
        private readonly AutoTallerDbContext _context;

        public UserRoleRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
            => await _context.UserRoles
                            .Include(x => x.Role)
                            .Where(x => x.UserId == userId)
                            .ToListAsync();

        public async Task<bool> ExistsAsync(int userId, int roleId)
            => await _context.UserRoles
                            .AnyAsync(x => x.UserId == userId
                                        && x.RoleId == roleId);

        public async Task AddAsync(UserRole userRole)
            => await _context.UserRoles.AddAsync(userRole);

        public void Remove(UserRole userRole)
            => _context.UserRoles.Remove(userRole);
    }
}
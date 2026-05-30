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
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly AutoTallerDbContext _context;

        public RoleRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
            => await _context.Roles
                            .Include(x => x.UserRoles)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Role>> GetAllAsync()
            => await _context.Roles.ToListAsync();

        public async Task<bool> ExistsByNameAsync(string roleName)
            => await _context.Roles.AnyAsync(x => x.RoleName.Value == roleName);

        public async Task AddAsync(Role role)
            => await _context.Roles.AddAsync(role);

        public void Update(Role role)
            => _context.Roles.Update(role);

        public void Remove(Role role)
            => _context.Roles.Remove(role);
    }
}
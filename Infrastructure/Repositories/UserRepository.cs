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
    public sealed class UserRepository : IUserRepository
    {
        private readonly AutoTallerDbContext _context;

        public UserRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users
                            .Include(x => x.Person)
                            .Include(x => x.UserRoles)
                                .ThenInclude(x => x.Role)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users
                            .Include(x => x.Person)
                            .Include(x => x.UserRoles)
                                .ThenInclude(x => x.Role)
                            .ToListAsync();

        public async Task<User?> GetByPersonIdAsync(int personId)
            => await _context.Users
                            .Include(x => x.Person)
                            .Include(x => x.UserRoles)
                                .ThenInclude(x => x.Role)
                            .FirstOrDefaultAsync(x => x.PersonId == personId);

        public async Task<bool> ExistsByPersonIdAsync(int personId)
            => await _context.Users.AnyAsync(x => x.PersonId == personId);

        public async Task AddAsync(User user)
            => await _context.Users.AddAsync(user);

        public void Update(User user)
            => _context.Users.Update(user);

        public void Remove(User user)
            => _context.Users.Remove(user);
    }
}

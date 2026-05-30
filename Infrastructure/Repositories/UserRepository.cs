using Application.Contracts.Repositories;
using Domain.Entities.Users;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public UserRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users
            .Include(x => x.Person)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .Include(x => x.Person)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .ToListAsync();
    }

    public async Task<User?> GetByPersonIdAsync(int personId)
    {
        return await _dbContext.Users
            .Include(x => x.Person)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.PersonId == personId);
    }

    public async Task<bool> ExistsByPersonIdAsync(int personId)
    {
        return await _dbContext.Users.AnyAsync(x => x.PersonId == personId);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public void Remove(User user)
    {
        _dbContext.Users.Remove(user);
    }
}

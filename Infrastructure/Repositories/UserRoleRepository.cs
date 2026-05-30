using Application.Contracts.Repositories;
using Domain.Entities.Users;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public UserRoleRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.UserRoles
            .Include(x => x.Role)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int userId, int roleId)
    {
        return await _dbContext.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
    }

    public async Task AddAsync(UserRole userRole)
    {
        await _dbContext.UserRoles.AddAsync(userRole);
    }

    public void Remove(UserRole userRole)
    {
        _dbContext.UserRoles.Remove(userRole);
    }
}

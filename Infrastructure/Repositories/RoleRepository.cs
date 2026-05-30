using Application.Contracts.Repositories;
using Domain.Entities.Users;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public RoleRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _dbContext.Roles
            .OrderBy(x => x.RoleName.Value)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string roleName)
    {
        var normalizedRoleName = roleName.Trim().ToLowerInvariant();
        var roles = await _dbContext.Roles.ToListAsync();
        return roles.Any(x => x.RoleName.Value.ToLowerInvariant() == normalizedRoleName);
    }

    public async Task AddAsync(Role role)
    {
        await _dbContext.Roles.AddAsync(role);
    }

    public void Update(Role role)
    {
        _dbContext.Roles.Update(role);
    }

    public void Remove(Role role)
    {
        _dbContext.Roles.Remove(role);
    }
}

using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Users;
using Application.Requests.Users;
using Domain.Entities.Users;
using Domain.ValueObject.Users.User;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly AutoTallerDbContext _dbContext;

    public UserService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        AutoTallerDbContext dbContext)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetActiveMechanicsAsync()
    {
        var activeUsers = await _dbContext.Users
            .Include(x => x.Person)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Where(x => x.IsActive)
            .ToListAsync();

        var mechanics = activeUsers
            .Where(x => x.UserRoles.Any(ur => ur.Role.RoleName.Value == "Mechanic"))
            .OrderBy(x => x.Person.FirstName.Value)
            .ThenBy(x => x.Person.LastName.Value);

        return mechanics.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        if (!await _dbContext.Persons.AnyAsync(x => x.Id == request.PersonId))
        {
            throw new ArgumentException($"Person {request.PersonId} does not exist.");
        }

        if (await _userRepository.ExistsByPersonIdAsync(request.PersonId))
        {
            throw new InvalidOperationException($"Person {request.PersonId} already has a user.");
        }

        var roleIds = await ValidateRoleIdsAsync(request.RoleIds);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.PersonId, new PasswordHash(passwordHash));

        await _userRepository.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        foreach (var roleId in roleIds)
        {
            await _userRoleRepository.AddAsync(new UserRole(user.Id, roleId));
        }

        await _dbContext.SaveChangesAsync();

        var createdUser = await _userRepository.GetByIdAsync(user.Id)
            ?? throw new InvalidOperationException("User could not be reloaded after creation.");

        return MapToDto(createdUser);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.UpdatePassword(new PasswordHash(BCrypt.Net.BCrypt.HashPassword(request.Password)));
        }

        if (request.IsActive)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        _userRepository.Update(user);

        var roleIds = await ValidateRoleIdsAsync(request.RoleIds);
        var existingRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);

        foreach (var existingRole in existingRoles.Where(x => !roleIds.Contains(x.RoleId)))
        {
            _userRoleRepository.Remove(existingRole);
        }

        foreach (var roleId in roleIds.Where(roleId => existingRoles.All(x => x.RoleId != roleId)))
        {
            await _userRoleRepository.AddAsync(new UserRole(user.Id, roleId));
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        _userRepository.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        user.Activate();
        _userRepository.Update(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        user.Deactivate();
        _userRepository.Update(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<HashSet<int>> ValidateRoleIdsAsync(IEnumerable<int> roleIds)
    {
        var distinctRoleIds = roleIds
            .Where(x => x > 0)
            .Distinct()
            .ToHashSet();

        if (distinctRoleIds.Count == 0)
        {
            throw new ArgumentException("At least one role must be assigned.");
        }

        var existingRoleIds = await _dbContext.Roles
            .Where(x => distinctRoleIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        if (existingRoleIds.Count != distinctRoleIds.Count)
        {
            throw new ArgumentException("One or more roles do not exist.");
        }

        return distinctRoleIds;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            PersonId = user.PersonId,
            FirstName = user.Person.FirstName.Value,
            LastName = user.Person.LastName.Value,
            IsActive = user.IsActive,
            Roles = user.UserRoles
                .Select(x => x.Role.RoleName.Value)
                .OrderBy(x => x)
                .ToArray()
        };
    }
}

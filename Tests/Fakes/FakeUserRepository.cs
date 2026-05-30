using Application.Contracts.Repositories;
using Domain.Entities.Users;

namespace AutoTallerManager.Tests.Fakes;

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly User? _userByEmail;

    public FakeUserRepository(User? userByEmail)
    {
        _userByEmail = userByEmail;
    }

    public Task<User?> GetByIdAsync(int id) => Task.FromResult<User?>(null);

    public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>([]);

    public Task<User?> GetByPersonIdAsync(int personId) => Task.FromResult<User?>(null);

    public Task<User?> GetByPrimaryEmailAsync(string emailUser, string emailDomain)
    {
        return Task.FromResult(_userByEmail);
    }

    public Task<bool> ExistsByPersonIdAsync(int personId) => Task.FromResult(false);

    public Task AddAsync(User user) => Task.CompletedTask;

    public void Update(User user)
    {
    }

    public void Remove(User user)
    {
    }
}

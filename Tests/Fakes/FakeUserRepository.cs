using Application.Contracts.Repositories;
using Domain.Entities.Users;

namespace AutoTallerManager.Tests.Fakes;

internal sealed class FakeUserRepository : IUserRepository
{
    // Dependencia compartida por varios escenarios de esta clase de pruebas.
    private readonly User? _userByEmail;

    public FakeUserRepository(User? userByEmail)
    {
        _userByEmail = userByEmail;
    }

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task<User?> GetByIdAsync(int id) => Task.FromResult<User?>(null);

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>([]);

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task<User?> GetByPersonIdAsync(int personId) => Task.FromResult<User?>(null);

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task<User?> GetByPrimaryEmailAsync(string emailUser, string emailDomain)
    {
        return Task.FromResult(_userByEmail);
    }

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task<bool> ExistsByPersonIdAsync(int personId) => Task.FromResult(false);

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public Task AddAsync(User user) => Task.CompletedTask;

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public void Update(User user)
    {
    }

    // Metodo de apoyo que simplifica la preparacion o reutilizacion del escenario.
    public void Remove(User user)
    {
    }
}

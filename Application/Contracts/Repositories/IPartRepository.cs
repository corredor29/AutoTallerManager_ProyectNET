using Domain.Entities.Parts;

namespace Application.Contracts.Repositories;

public interface IPartRepository
{
    Task<Part?> GetByIdAsync(int id);
    Task<IEnumerable<Part>> GetAllAsync();
    Task AddAsync(Part part);
    void Update(Part part);
    void Remove(Part part);
}

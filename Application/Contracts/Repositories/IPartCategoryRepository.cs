using Domain.Entities.Parts;

namespace Application.Contracts.Repositories;

public interface IPartCategoryRepository
{
    Task<PartCategory?> GetByIdAsync(int id);
    Task<IEnumerable<PartCategory>> GetAllAsync();
    Task AddAsync(PartCategory partCategory);
    void Update(PartCategory partCategory);
    void Remove(PartCategory partCategory);
}

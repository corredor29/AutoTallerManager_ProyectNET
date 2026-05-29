using Application.DTOs.Parts;
using Application.Requests.Parts;

namespace Application.Contracts.Services;

public interface IPartService
{
    Task<IEnumerable<PartDto>> GetAllAsync();
    Task<PartDto?> GetByIdAsync(int id);
    Task<PartDto> CreateAsync(CreatePartRequest request);
    Task<bool> UpdateAsync(int id, UpdatePartRequest request);
    Task<bool> DeleteAsync(int id);
}

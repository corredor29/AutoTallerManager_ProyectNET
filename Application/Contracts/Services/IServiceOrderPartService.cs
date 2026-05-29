using Application.DTOs.ServiceOrderParts;
using Application.Requests.ServiceOrderParts;

namespace Application.Contracts.Services;

public interface IServiceOrderPartService
{
    Task<IEnumerable<ServiceOrderPartDto>> GetAllAsync();
    Task<ServiceOrderPartDto?> GetByIdAsync(int id);
    Task<ServiceOrderPartDto> CreateAsync(CreateServiceOrderPartRequest request);
    Task<bool> UpdateAsync(int id, UpdateServiceOrderPartRequest request);
    Task<bool> DeleteAsync(int id);
}

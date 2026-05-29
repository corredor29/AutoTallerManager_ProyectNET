using Application.DTOs.ServiceTypes;
using Application.Requests.ServiceTypes;

namespace Application.Contracts.Services;

public interface IServiceTypeService
{
    Task<IEnumerable<ServiceTypeDto>> GetAllAsync();
    Task<ServiceTypeDto?> GetByIdAsync(int id);
    Task<ServiceTypeDto> CreateAsync(CreateServiceTypeRequest request);
    Task<bool> UpdateAsync(int id, UpdateServiceTypeRequest request);
    Task<bool> DeleteAsync(int id);
}

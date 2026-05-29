using Application.DTOs.PartSuppliers;
using Application.Requests.PartSuppliers;

namespace Application.Contracts.Services;

public interface IPartSupplierService
{
    Task<IEnumerable<PartSupplierDto>> GetAllAsync();
    Task<PartSupplierDto?> GetByIdAsync(int id);
    Task<PartSupplierDto> CreateAsync(CreatePartSupplierRequest request);
    Task<bool> UpdateAsync(int id, UpdatePartSupplierRequest request);
    Task<bool> DeleteAsync(int id);
}

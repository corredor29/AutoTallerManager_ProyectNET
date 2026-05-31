using Application.Common.Pagination;
using Application.DTOs.Parts;
using Application.Filters;
using Application.Requests.Parts;

namespace Application.Contracts.Services;

public interface IPartService
{
    Task<PagedResult<PartDto>> GetAllPagedAsync(PaginationParams pagination, PartFilter filter);
    Task<PartDto?> GetByIdAsync(int id);
    Task<PartDto> CreateAsync(CreatePartRequest request);
    Task<bool> UpdateAsync(int id, UpdatePartRequest request);
    Task<bool> DeleteAsync(int id);
}

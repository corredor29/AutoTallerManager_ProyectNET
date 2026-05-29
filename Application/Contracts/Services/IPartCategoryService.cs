using Application.DTOs.PartCategories;
using Application.Requests.PartCategories;

namespace Application.Contracts.Services;

public interface IPartCategoryService
{
    Task<IEnumerable<PartCategoryDto>> GetAllAsync();
    Task<PartCategoryDto?> GetByIdAsync(int id);
    Task<PartCategoryDto> CreateAsync(CreatePartCategoryRequest request);
    Task<bool> UpdateAsync(int id, UpdatePartCategoryRequest request);
    Task<bool> DeleteAsync(int id);
}

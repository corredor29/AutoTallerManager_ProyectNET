using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PartCategories;
using Application.Requests.PartCategories;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.PartCategory;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PartCategoryService : IPartCategoryService
{
    private readonly IPartCategoryRepository _partCategoryRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PartCategoryService(IPartCategoryRepository partCategoryRepository, AutoTallerDbContext dbContext)
    {
        _partCategoryRepository = partCategoryRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PartCategoryDto>> GetAllAsync()
    {
        var categories = await _partCategoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<PartCategoryDto?> GetByIdAsync(int id)
    {
        var category = await _partCategoryRepository.GetByIdAsync(id);
        return category is null ? null : MapToDto(category);
    }

    public async Task<PartCategoryDto> CreateAsync(CreatePartCategoryRequest request)
    {
        await EnsureNameIsUniqueAsync(request.Name);

        var category = new PartCategory(new PartCategoryName(request.Name));
        await _partCategoryRepository.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePartCategoryRequest request)
    {
        var category = await _partCategoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }

        await EnsureNameIsUniqueAsync(request.Name, id);

        category.Update(new PartCategoryName(request.Name));
        _partCategoryRepository.Update(category);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _partCategoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }

        if (await _dbContext.Parts.AnyAsync(x => x.PartCategoryId == id))
        {
            throw new InvalidOperationException($"Part category {id} is being used and cannot be deleted.");
        }

        _partCategoryRepository.Remove(category);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await _dbContext.PartCategories.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Part category '{name}' already exists.");
        }
    }

    private static PartCategoryDto MapToDto(PartCategory category)
    {
        return new PartCategoryDto
        {
            Id = category.Id,
            Name = category.Name.Value
        };
    }
}

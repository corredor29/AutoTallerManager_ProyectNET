using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.QuotationStatuses;
using Application.Requests.QuotationStatuses;
using Domain.Entities.Quotations;
using Domain.ValueObject.Quotations.QuotationStatus;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class QuotationStatusService : IQuotationStatusService
{
    private readonly IQuotationStatusRepository _quotationStatusRepository;
    private readonly AutoTallerDbContext _dbContext;

    public QuotationStatusService(IQuotationStatusRepository quotationStatusRepository, AutoTallerDbContext dbContext)
    {
        _quotationStatusRepository = quotationStatusRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<QuotationStatusDto>> GetAllAsync()
    {
        var statuses = await _quotationStatusRepository.GetAllAsync();
        return statuses.Select(MapToDto);
    }

    public async Task<QuotationStatusDto?> GetByIdAsync(int id)
    {
        var status = await _quotationStatusRepository.GetByIdAsync(id);
        return status is null ? null : MapToDto(status);
    }

    public async Task<QuotationStatusDto> CreateAsync(CreateQuotationStatusRequest request)
    {
        await EnsureNameIsUniqueAsync(request.Name);

        var status = new QuotationStatus(new QuotationStatusName(request.Name));
        await _quotationStatusRepository.AddAsync(status);
        await _dbContext.SaveChangesAsync();

        return MapToDto(status);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationStatusRequest request)
    {
        var status = await _quotationStatusRepository.GetByIdAsync(id);
        if (status is null)
        {
            return false;
        }

        await EnsureNameIsUniqueAsync(request.Name, id);

        status.Update(new QuotationStatusName(request.Name));
        _quotationStatusRepository.Update(status);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var status = await _quotationStatusRepository.GetByIdAsync(id);
        if (status is null)
        {
            return false;
        }

        if (await _dbContext.Quotations.AnyAsync(x => x.QuotationStatusId == id))
        {
            throw new InvalidOperationException($"Quotation status {id} is being used and cannot be deleted.");
        }

        _quotationStatusRepository.Remove(status);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await _dbContext.QuotationStatuses.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Quotation status '{name}' already exists.");
        }
    }

    private static QuotationStatusDto MapToDto(QuotationStatus status)
    {
        return new QuotationStatusDto
        {
            Id = status.Id,
            Name = status.Name.Value
        };
    }
}

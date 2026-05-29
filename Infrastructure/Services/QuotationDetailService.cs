using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.QuotationDetails;
using Application.Requests.QuotationDetails;
using Domain.Entities.Quotations;
using Domain.ValueObject.Quotations.QuotationDetail;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class QuotationDetailService : IQuotationDetailService
{
    private readonly IQuotationDetailRepository _quotationDetailRepository;
    private readonly AutoTallerDbContext _dbContext;

    public QuotationDetailService(IQuotationDetailRepository quotationDetailRepository, AutoTallerDbContext dbContext)
    {
        _quotationDetailRepository = quotationDetailRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<QuotationDetailDto>> GetAllAsync()
    {
        var details = await _quotationDetailRepository.GetAllAsync();
        return details.Select(MapToDto);
    }

    public async Task<QuotationDetailDto?> GetByIdAsync(int id)
    {
        var detail = await _quotationDetailRepository.GetByIdAsync(id);
        return detail is null ? null : MapToDto(detail);
    }

    public async Task<QuotationDetailDto> CreateAsync(CreateQuotationDetailRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.QuotationId, request.PartId);
        await EnsurePartIsNotRepeatedAsync(request.QuotationId, request.PartId);

        var detail = new QuotationDetail(
            request.QuotationId,
            request.PartId,
            new QuotationQuantity(request.Quantity),
            new QuotationUnitPrice(request.UnitPrice));

        await _quotationDetailRepository.AddAsync(detail);
        await _dbContext.SaveChangesAsync();

        var createdDetail = await _quotationDetailRepository.GetByIdAsync(detail.Id)
            ?? throw new InvalidOperationException("Quotation detail could not be reloaded after creation.");

        return MapToDto(createdDetail);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationDetailRequest request)
    {
        var detail = await _quotationDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        detail.Update(
            new QuotationQuantity(request.Quantity),
            new QuotationUnitPrice(request.UnitPrice));

        _quotationDetailRepository.Update(detail);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var detail = await _quotationDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        _quotationDetailRepository.Remove(detail);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int quotationId, int partId)
    {
        if (!await _dbContext.Quotations.AnyAsync(x => x.Id == quotationId))
        {
            throw new ArgumentException($"Quotation {quotationId} does not exist.");
        }

        if (!await _dbContext.Parts.AnyAsync(x => x.Id == partId && x.IsActive))
        {
            throw new ArgumentException($"Part {partId} does not exist or is inactive.");
        }
    }

    private async Task EnsurePartIsNotRepeatedAsync(int quotationId, int partId)
    {
        var exists = await _dbContext.QuotationDetails.AnyAsync(x =>
            x.QuotationId == quotationId &&
            x.PartId == partId);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Part {partId} is already assigned to quotation {quotationId}.");
        }
    }

    private static QuotationDetailDto MapToDto(QuotationDetail detail)
    {
        return new QuotationDetailDto
        {
            Id = detail.Id,
            QuotationId = detail.QuotationId,
            PartId = detail.PartId,
            PartCode = detail.Part.Code.Value,
            PartDescription = detail.Part.Description.Value,
            Quantity = detail.Quantity.Value,
            UnitPrice = detail.UnitPrice.Value,
            LineTotal = detail.Quantity.Value * detail.UnitPrice.Value
        };
    }
}

using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Quotations;
using Application.Requests.Quotations;
using Domain.Entities.Quotations;
using Domain.ValueObject.Quotations.Quotation;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly AutoTallerDbContext _dbContext;

    public QuotationService(IQuotationRepository quotationRepository, AutoTallerDbContext dbContext)
    {
        _quotationRepository = quotationRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<QuotationDto>> GetAllAsync()
    {
        var quotations = await _quotationRepository.GetAllAsync();
        return quotations.Select(MapToDto);
    }

    public async Task<QuotationDto?> GetByIdAsync(int id)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        return quotation is null ? null : MapToDto(quotation);
    }

    public async Task<QuotationDto> CreateAsync(CreateQuotationRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(
            request.ServiceOrderId,
            request.CreatedByUserId,
            request.QuotationStatusId);

        var quotation = new Quotation(
            request.ServiceOrderId,
            request.CreatedByUserId,
            request.QuotationStatusId,
            new LaborCost(request.LaborCost),
            new QuotationSubtotal(request.Subtotal),
            new QuotationTotal(request.Total),
            new QuotationNotes(request.Notes));

        await _quotationRepository.AddAsync(quotation);
        await _dbContext.SaveChangesAsync();

        var createdQuotation = await _quotationRepository.GetByIdAsync(quotation.Id)
            ?? throw new InvalidOperationException("Quotation could not be reloaded after creation.");

        return MapToDto(createdQuotation);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationRequest request)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null)
        {
            return false;
        }

        quotation.Update(
            new LaborCost(request.LaborCost),
            new QuotationSubtotal(request.Subtotal),
            new QuotationTotal(request.Total),
            new QuotationNotes(request.Notes));

        _quotationRepository.Update(quotation);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangeQuotationStatusRequest request)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null)
        {
            return false;
        }

        var statusName = await _dbContext.QuotationStatuses
            .Where(x => x.Id == request.QuotationStatusId)
            .Select(x => x.Name.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(statusName))
        {
            throw new ArgumentException($"Quotation status {request.QuotationStatusId} does not exist.");
        }

        quotation.ChangeStatus(request.QuotationStatusId);

        if (statusName.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
        {
            quotation.Accept();
        }
        else if (statusName.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        {
            quotation.Reject(new RejectionReason(request.RejectionReason));
        }

        _quotationRepository.Update(quotation);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null)
        {
            return false;
        }

        _quotationRepository.Remove(quotation);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int serviceOrderId, int createdByUserId, int quotationStatusId)
    {
        if (!await _dbContext.ServiceOrders.AnyAsync(x => x.Id == serviceOrderId))
        {
            throw new ArgumentException($"Service order {serviceOrderId} does not exist.");
        }

        if (!await _dbContext.Users.AnyAsync(x => x.Id == createdByUserId && x.IsActive))
        {
            throw new ArgumentException($"User {createdByUserId} does not exist or is inactive.");
        }

        if (!await _dbContext.QuotationStatuses.AnyAsync(x => x.Id == quotationStatusId))
        {
            throw new ArgumentException($"Quotation status {quotationStatusId} does not exist.");
        }
    }

    private static QuotationDto MapToDto(Quotation quotation)
    {
        var createdByUserName = quotation.CreatedByUser?.Person is null
            ? string.Empty
            : $"{quotation.CreatedByUser.Person.FirstName.Value} {quotation.CreatedByUser.Person.LastName.Value}".Trim();

        return new QuotationDto
        {
            Id = quotation.Id,
            ServiceOrderId = quotation.ServiceOrderId,
            CreatedByUserId = quotation.CreatedByUserId,
            CreatedByUserName = createdByUserName,
            QuotationStatusId = quotation.QuotationStatusId,
            QuotationStatusName = quotation.QuotationStatus?.Name.Value ?? string.Empty,
            CreatedAt = quotation.CreatedAt,
            RespondedAt = quotation.RespondedAt,
            LaborCost = quotation.LaborCost.Value,
            Subtotal = quotation.Subtotal.Value,
            Total = quotation.Total.Value,
            RejectionReason = quotation.RejectionReason.Value,
            Notes = quotation.Notes.Value
        };
    }
}

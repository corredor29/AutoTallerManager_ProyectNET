using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Notifications;
using Application.DTOs.Quotations;
using Application.Requests.Quotations;
using Domain.Entities.Invoices;
using Domain.Entities.Quotations;
using Domain.ValueObject.Invoices.Invoice;
using Domain.ValueObject.Quotations.Quotation;
using Infrastructure.Context;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class QuotationService : IQuotationService
{
    private readonly IQuotationRepository        _quotationRepository;
    private readonly AutoTallerDbContext          _dbContext;
    private readonly IHubContext<NotificationHub> _hub;

    public QuotationService(
        IQuotationRepository quotationRepository,
        AutoTallerDbContext dbContext,
        IHubContext<NotificationHub> hub)
    {
        _quotationRepository = quotationRepository;
        _dbContext           = dbContext;
        _hub                 = hub;
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

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "create",
            Entity     = "Quotation",
            RecordId   = quotation.Id,
            Message    = $"New quotation #Q-{quotation.Id} created for order #SO-{request.ServiceOrderId}",
            OccurredAt = DateTime.UtcNow
        });

        return MapToDto(createdQuotation);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationRequest request)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null) return false;

        quotation.Update(
            new LaborCost(request.LaborCost),
            new QuotationSubtotal(request.Subtotal),
            new QuotationTotal(request.Total),
            new QuotationNotes(request.Notes));

        _quotationRepository.Update(quotation);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "update",
            Entity     = "Quotation",
            RecordId   = id,
            Message    = $"Quotation #Q-{id} updated",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangeQuotationStatusRequest request)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null) return false;

        var allStatuses = await _dbContext.QuotationStatuses.ToListAsync();
        var status      = allStatuses.FirstOrDefault(x => x.Id == request.QuotationStatusId);

        if (status is null)
            throw new ArgumentException($"Quotation status {request.QuotationStatusId} does not exist.");

        var statusName = status.Name.Value;

        quotation.ChangeStatus(request.QuotationStatusId);

        if (statusName.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
            quotation.Accept();
        else if (statusName.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        {
            quotation.Reject(new RejectionReason(request.RejectionReason));
            await CreateDiagnosisOnlyInvoiceAsync(quotation);
        }

        _quotationRepository.Update(quotation);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "status",
            Entity     = "Quotation",
            RecordId   = id,
            Message    = $"Quotation #Q-{id} status changed to {statusName}",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var quotation = await _quotationRepository.GetByIdAsync(id);
        if (quotation is null) return false;

        _quotationRepository.Remove(quotation);
        await _dbContext.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("Notification", new NotificationDto
        {
            Type       = "delete",
            Entity     = "Quotation",
            RecordId   = id,
            Message    = $"Quotation #Q-{id} deleted",
            OccurredAt = DateTime.UtcNow
        });

        return true;
    }

    private async Task CreateDiagnosisOnlyInvoiceAsync(Quotation quotation)
    {
        var alreadyHasInvoice = await _dbContext.Invoices
            .AnyAsync(x => x.ServiceOrderId == quotation.ServiceOrderId);

        if (alreadyHasInvoice) return;

        var diagnosticCost = quotation.LaborCost?.Value ?? 0;

        var invoice = new Invoice(
            quotation.ServiceOrderId,
            new InvoiceLaborCost(diagnosticCost),
            new InvoiceSubtotal(diagnosticCost),
            new InvoiceTax(0m),
            new InvoiceTotal(diagnosticCost),
            diagnosisOnlyCharged: true,
            quotationId: quotation.Id);

        await _dbContext.Invoices.AddAsync(invoice);
    }

    private async Task EnsureRelatedEntitiesExistAsync(
        int serviceOrderId, int createdByUserId, int quotationStatusId)
    {
        if (!await _dbContext.ServiceOrders.AnyAsync(x => x.Id == serviceOrderId))
            throw new ArgumentException($"Service order {serviceOrderId} does not exist.");

        if (!await _dbContext.Users.AnyAsync(x => x.Id == createdByUserId && x.IsActive))
            throw new ArgumentException($"User {createdByUserId} does not exist or is inactive.");

        if (!await _dbContext.QuotationStatuses.AnyAsync(x => x.Id == quotationStatusId))
            throw new ArgumentException($"Quotation status {quotationStatusId} does not exist.");
    }

    private static QuotationDto MapToDto(Quotation quotation)
    {
        var createdByUserName = quotation.CreatedByUser?.Person is null
            ? string.Empty
            : $"{quotation.CreatedByUser.Person.FirstName.Value} {quotation.CreatedByUser.Person.LastName.Value}".Trim();

        return new QuotationDto
        {
            Id                  = quotation.Id,
            ServiceOrderId      = quotation.ServiceOrderId,
            CreatedByUserId     = quotation.CreatedByUserId,
            CreatedByUserName   = createdByUserName,
            QuotationStatusId   = quotation.QuotationStatusId,
            QuotationStatusName = quotation.QuotationStatus?.Name.Value ?? string.Empty,
            CreatedAt           = quotation.CreatedAt,
            RespondedAt         = quotation.RespondedAt,
            LaborCost           = quotation.LaborCost?.Value    ?? 0,
            Subtotal            = quotation.Subtotal?.Value     ?? 0,
            Total               = quotation.Total?.Value        ?? 0,
            RejectionReason     = quotation.RejectionReason?.Value,
            Notes               = quotation.Notes?.Value
        };
    }
}
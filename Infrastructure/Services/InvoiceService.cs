using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Invoices;
using Application.Requests.Invoices;
using Domain.Entities.Invoices;
using Domain.ValueObject.Invoices.Invoice;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceService(IInvoiceRepository invoiceRepository, AutoTallerDbContext dbContext)
    {
        _invoiceRepository = invoiceRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        return invoices.Select(MapToDto);
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        return invoice is null ? null : MapToDto(invoice);
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.ServiceOrderId, request.QuotationId);
        await EnsureUniqueServiceOrderInvoiceAsync(request.ServiceOrderId);
        await EnsureQuotationAvailabilityAsync(request.QuotationId);

        var subtotal = request.LaborCost;
        var total = subtotal + request.Tax;

        var invoice = new Invoice(
            request.ServiceOrderId,
            new InvoiceLaborCost(request.LaborCost),
            new InvoiceSubtotal(subtotal),
            new InvoiceTax(request.Tax),
            new InvoiceTotal(total),
            request.DiagnosisOnlyCharged,
            request.QuotationId);

        await _invoiceRepository.AddAsync(invoice);
        await _dbContext.SaveChangesAsync();

        return MapToDto(invoice);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvoiceRequest request)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null)
        {
            return false;
        }

        var detailsTotal = await _dbContext.InvoiceDetails
            .Where(x => x.InvoiceId == id)
            .SumAsync(x => x.Quantity.Value * x.UnitPrice.Value);

        var subtotal = request.LaborCost + detailsTotal;
        var total = subtotal + request.Tax;

        invoice.Update(
            new InvoiceLaborCost(request.LaborCost),
            new InvoiceSubtotal(subtotal),
            new InvoiceTax(request.Tax),
            new InvoiceTotal(total),
            request.DiagnosisOnlyCharged);

        _invoiceRepository.Update(invoice);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null)
        {
            return false;
        }

        if (await _dbContext.InvoiceDetails.AnyAsync(x => x.InvoiceId == id))
        {
            throw new InvalidOperationException($"Invoice {id} has details and cannot be deleted.");
        }

        _invoiceRepository.Remove(invoice);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int serviceOrderId, int? quotationId)
    {
        if (!await _dbContext.ServiceOrders.AnyAsync(x => x.Id == serviceOrderId))
        {
            throw new ArgumentException($"Service order {serviceOrderId} does not exist.");
        }

        if (quotationId.HasValue && !await _dbContext.Quotations.AnyAsync(x => x.Id == quotationId.Value))
        {
            throw new ArgumentException($"Quotation {quotationId.Value} does not exist.");
        }
    }

    private async Task EnsureUniqueServiceOrderInvoiceAsync(int serviceOrderId)
    {
        if (await _dbContext.Invoices.AnyAsync(x => x.ServiceOrderId == serviceOrderId))
        {
            throw new InvalidOperationException($"Service order {serviceOrderId} already has an invoice.");
        }
    }

    private async Task EnsureQuotationAvailabilityAsync(int? quotationId)
    {
        if (!quotationId.HasValue)
        {
            return;
        }

        if (await _dbContext.Invoices.AnyAsync(x => x.QuotationId == quotationId.Value))
        {
            throw new InvalidOperationException($"Quotation {quotationId.Value} is already linked to another invoice.");
        }
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            ServiceOrderId = invoice.ServiceOrderId,
            QuotationId = invoice.QuotationId,
            IssuedAt = invoice.IssuedAt,
            LaborCost = invoice.LaborCost.Value,
            Subtotal = invoice.Subtotal.Value,
            Tax = invoice.Tax.Value,
            Total = invoice.Total.Value,
            DiagnosisOnlyCharged = invoice.DiagnosisOnlyCharged
        };
    }
}

using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.InvoiceDetails;
using Application.Requests.InvoiceDetails;
using Domain.Entities.Invoices;
using Domain.ValueObject.Invoices.Invoice;
using Domain.ValueObject.Invoices.InvoiceDetail;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class InvoiceDetailService : IInvoiceDetailService
{
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceDetailService(IInvoiceDetailRepository invoiceDetailRepository, AutoTallerDbContext dbContext)
    {
        _invoiceDetailRepository = invoiceDetailRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<InvoiceDetailDto>> GetAllAsync()
    {
        var details = await _invoiceDetailRepository.GetAllAsync();
        return details.Select(MapToDto);
    }

    public async Task<InvoiceDetailDto?> GetByIdAsync(int id)
    {
        var detail = await _invoiceDetailRepository.GetByIdAsync(id);
        return detail is null ? null : MapToDto(detail);
    }

    public async Task<InvoiceDetailDto> CreateAsync(CreateInvoiceDetailRequest request)
    {
        await EnsureInvoiceExistsAsync(request.InvoiceId);

        var detail = new InvoiceDetail(
            request.InvoiceId,
            new InvoiceDetailDescription(request.Description),
            new InvoiceDetailQuantity(request.Quantity),
            new InvoiceDetailUnitPrice(request.UnitPrice));

        await _invoiceDetailRepository.AddAsync(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculateInvoiceTotalsAsync(request.InvoiceId);

        return MapToDto(detail);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvoiceDetailRequest request)
    {
        var detail = await _invoiceDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        detail.Update(
            new InvoiceDetailDescription(request.Description),
            new InvoiceDetailQuantity(request.Quantity),
            new InvoiceDetailUnitPrice(request.UnitPrice));

        _invoiceDetailRepository.Update(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculateInvoiceTotalsAsync(detail.InvoiceId);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var detail = await _invoiceDetailRepository.GetByIdAsync(id);
        if (detail is null)
        {
            return false;
        }

        var invoiceId = detail.InvoiceId;
        _invoiceDetailRepository.Remove(detail);
        await _dbContext.SaveChangesAsync();
        await RecalculateInvoiceTotalsAsync(invoiceId);
        return true;
    }

    private async Task EnsureInvoiceExistsAsync(int invoiceId)
    {
        if (!await _dbContext.Invoices.AnyAsync(x => x.Id == invoiceId))
        {
            throw new ArgumentException($"Invoice {invoiceId} does not exist.");
        }
    }

    private async Task RecalculateInvoiceTotalsAsync(int invoiceId)
    {
        var invoice = await _dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == invoiceId)
            ?? throw new InvalidOperationException($"Invoice {invoiceId} could not be loaded.");

        var detailsTotal = await _dbContext.InvoiceDetails
            .Where(x => x.InvoiceId == invoiceId)
            .SumAsync(x => x.Quantity.Value * x.UnitPrice.Value);

        var subtotal = invoice.LaborCost.Value + detailsTotal;
        var total = subtotal + invoice.Tax.Value;

        invoice.Update(
            invoice.LaborCost,
            new InvoiceSubtotal(subtotal),
            invoice.Tax,
            new InvoiceTotal(total),
            invoice.DiagnosisOnlyCharged);

        await _dbContext.SaveChangesAsync();
    }

    private static InvoiceDetailDto MapToDto(InvoiceDetail detail)
    {
        return new InvoiceDetailDto
        {
            Id = detail.Id,
            InvoiceId = detail.InvoiceId,
            Description = detail.Description.Value,
            Quantity = detail.Quantity.Value,
            UnitPrice = detail.UnitPrice.Value,
            LineTotal = detail.Quantity.Value * detail.UnitPrice.Value
        };
    }
}

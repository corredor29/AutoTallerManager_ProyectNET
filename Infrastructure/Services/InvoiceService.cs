using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Invoices;
using Application.Filters;
using Application.Requests.Invoices;
using Domain.Entities.Invoices;
using Domain.ValueObject.Invoices.Invoice;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class InvoiceService : IInvoiceService
{
    private static readonly string[] BillableStatuses = ["completed", "cancelled", "canceled"];

    private readonly IInvoiceRepository _invoiceRepository;
    private readonly AutoTallerDbContext _dbContext;

    public InvoiceService(IInvoiceRepository invoiceRepository, AutoTallerDbContext dbContext)
    {
        _invoiceRepository = invoiceRepository;
        _dbContext         = dbContext;
    }

    public async Task<PagedResult<InvoiceDto>> GetAllPagedAsync(
        PaginationParams pagination, InvoiceFilter filter)
    {
        var result = await _invoiceRepository.GetAllPagedAsync(pagination, filter);
        return new PagedResult<InvoiceDto>
        {
            Items      = result.Items.Select(MapToDto).ToArray(),
            PageNumber = result.PageNumber,
            PageSize   = result.PageSize,
            TotalCount = result.TotalCount
        };
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

        var serviceOrderParts = await _dbContext.ServiceOrderParts
            .Include(x => x.Part)
            .Where(x => x.ServiceOrderId == request.ServiceOrderId)
            .OrderBy(x => x.Id)
            .ToListAsync();

        var partsTotal = serviceOrderParts.Sum(x => x.Quantity.Value * x.AppliedUnitPrice.Value);
        var subtotal   = request.LaborCost + partsTotal;
        var total      = subtotal + request.Tax;

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

        if (serviceOrderParts.Count > 0)
        {
            var details = serviceOrderParts.Select(sp =>
                new InvoiceDetail(
                    invoice.Id,
                    new Domain.ValueObject.Invoices.InvoiceDetail.InvoiceDetailDescription(sp.Part.Description.Value),
                    new Domain.ValueObject.Invoices.InvoiceDetail.InvoiceDetailQuantity(sp.Quantity.Value),
                    new Domain.ValueObject.Invoices.InvoiceDetail.InvoiceDetailUnitPrice(sp.AppliedUnitPrice.Value)))
                .ToArray();

            await _dbContext.InvoiceDetails.AddRangeAsync(details);
            await _dbContext.SaveChangesAsync();
        }

        var createdInvoice = await _invoiceRepository.GetByIdAsync(invoice.Id)
            ?? throw new InvalidOperationException("Invoice could not be reloaded after creation.");

        return MapToDto(createdInvoice);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvoiceRequest request)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice is null) return false;

        var detailsTotal = await _dbContext.InvoiceDetails
            .Where(x => x.InvoiceId == id)
            .SumAsync(x => x.Quantity.Value * x.UnitPrice.Value);

        var subtotal = request.LaborCost + detailsTotal;
        var total    = subtotal + request.Tax;

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
        if (invoice is null) return false;

        if (await _dbContext.InvoiceDetails.AnyAsync(x => x.InvoiceId == id))
            throw new InvalidOperationException($"Invoice {id} has details and cannot be deleted.");

        _invoiceRepository.Remove(invoice);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    // ── Helpers ────────────────────────────────────
    private async Task EnsureRelatedEntitiesExistAsync(int serviceOrderId, int? quotationId)
    {
        // ── Fix: ToListAsync + filtro en memoria ───────
        var allOrders    = await _dbContext.ServiceOrders.Include(x => x.OrderStatus).ToListAsync();
        var serviceOrder = allOrders.FirstOrDefault(x => x.Id == serviceOrderId);

        if (serviceOrder is null)
            throw new ArgumentException($"Service order {serviceOrderId} does not exist.");

        var statusName = serviceOrder.OrderStatus?.Name.Value.ToLower() ?? string.Empty;
        if (!BillableStatuses.Contains(statusName))
            throw new InvalidOperationException($"Service order {serviceOrderId} is not ready to be invoiced.");

        if (quotationId.HasValue && !await _dbContext.Quotations.AnyAsync(x => x.Id == quotationId.Value))
            throw new ArgumentException($"Quotation {quotationId.Value} does not exist.");

        if (quotationId.HasValue)
        {
            var belongs = await _dbContext.Quotations.AnyAsync(x =>
                x.Id == quotationId.Value && x.ServiceOrderId == serviceOrderId);

            if (!belongs)
                throw new InvalidOperationException(
                    $"Quotation {quotationId.Value} does not belong to service order {serviceOrderId}.");
        }
    }

    private async Task EnsureUniqueServiceOrderInvoiceAsync(int serviceOrderId)
    {
        if (await _dbContext.Invoices.AnyAsync(x => x.ServiceOrderId == serviceOrderId))
            throw new InvalidOperationException($"Service order {serviceOrderId} already has an invoice.");
    }

    private async Task EnsureQuotationAvailabilityAsync(int? quotationId)
    {
        if (!quotationId.HasValue) return;

        if (await _dbContext.Invoices.AnyAsync(x => x.QuotationId == quotationId.Value))
            throw new InvalidOperationException(
                $"Quotation {quotationId.Value} is already linked to another invoice.");
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        var customerName = string.Empty;
        int? customerId  = null;

        if (invoice.ServiceOrder?.Appointment?.Customer?.Person is not null)
        {
            var p = invoice.ServiceOrder.Appointment.Customer.Person;
            customerName = $"{p.FirstName.Value} {p.LastName.Value}".Trim();
            customerId   = invoice.ServiceOrder.Appointment.CustomerId;
        }

        if (string.IsNullOrEmpty(customerName))
        {
            var owner = invoice.ServiceOrder?.Vehicle?.Ownerships?
                .FirstOrDefault(o => o.DateRange.EndDate == null);

            if (owner?.Customer?.Person is not null)
            {
                var p = owner.Customer.Person;
                customerName = $"{p.FirstName.Value} {p.LastName.Value}".Trim();
                customerId   = owner.CustomerId;
            }
        }

        return new InvoiceDto
        {
            Id                   = invoice.Id,
            ServiceOrderId       = invoice.ServiceOrderId,
            CustomerId           = customerId,
            CustomerName         = customerName,
            VehicleVin           = invoice.ServiceOrder?.Vehicle?.VIN.Value ?? string.Empty,
            QuotationId          = invoice.QuotationId,
            IssuedAt             = invoice.IssuedAt,
            LaborCost            = invoice.LaborCost?.Value ?? 0,
            Subtotal             = invoice.Subtotal?.Value  ?? 0,
            Tax                  = invoice.Tax?.Value       ?? 0,
            Total                = invoice.Total?.Value     ?? 0,
            DiagnosisOnlyCharged = invoice.DiagnosisOnlyCharged
        };
    }
}
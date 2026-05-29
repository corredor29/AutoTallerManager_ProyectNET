using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Payments;
using Application.Requests.Payments;
using Domain.Entities.Invoices;
using Domain.ValueObject.Invoices.Payment;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PaymentService(IPaymentRepository paymentRepository, AutoTallerDbContext dbContext)
    {
        _paymentRepository = paymentRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        return payment is null ? null : MapToDto(payment);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request)
    {
        await EnsureRelatedEntitiesExistAsync(request.InvoiceId, request.PaymentMethodId);
        await EnsurePaymentFitsInvoiceAsync(request.InvoiceId, request.Amount);

        var payment = new Payment(
            request.InvoiceId,
            request.PaymentMethodId,
            new PaymentAmount(request.Amount),
            new PaymentReference(request.Reference));

        await _paymentRepository.AddAsync(payment);
        await _dbContext.SaveChangesAsync();

        var createdPayment = await _paymentRepository.GetByIdAsync(payment.Id)
            ?? throw new InvalidOperationException("Payment could not be reloaded after creation.");

        return MapToDto(createdPayment);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePaymentRequest request)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment is null)
        {
            return false;
        }

        await EnsurePaymentFitsInvoiceAsync(payment.InvoiceId, request.Amount, id);

        payment.Update(
            new PaymentAmount(request.Amount),
            new PaymentReference(request.Reference));

        _paymentRepository.Update(payment);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment is null)
        {
            return false;
        }

        _paymentRepository.Remove(payment);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureRelatedEntitiesExistAsync(int invoiceId, int paymentMethodId)
    {
        if (!await _dbContext.Invoices.AnyAsync(x => x.Id == invoiceId))
        {
            throw new ArgumentException($"Invoice {invoiceId} does not exist.");
        }

        if (!await _dbContext.PaymentMethods.AnyAsync(x => x.Id == paymentMethodId))
        {
            throw new ArgumentException($"Payment method {paymentMethodId} does not exist.");
        }
    }

    private async Task EnsurePaymentFitsInvoiceAsync(int invoiceId, decimal amount, int? excludePaymentId = null)
    {
        var invoiceTotal = await _dbContext.Invoices
            .Where(x => x.Id == invoiceId)
            .Select(x => x.Total.Value)
            .FirstOrDefaultAsync();

        var paidAmount = await _dbContext.Payments
            .Where(x => x.InvoiceId == invoiceId && (!excludePaymentId.HasValue || x.Id != excludePaymentId.Value))
            .SumAsync(x => x.Amount.Value);

        if (paidAmount + amount > invoiceTotal)
        {
            throw new InvalidOperationException(
                $"Payment exceeds the pending balance for invoice {invoiceId}.");
        }
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            PaymentMethodId = payment.PaymentMethodId,
            PaymentMethodName = payment.PaymentMethod.Name.Value,
            Amount = payment.Amount.Value,
            PaidAt = payment.PaidAt,
            Reference = payment.Reference.Value
        };
    }
}

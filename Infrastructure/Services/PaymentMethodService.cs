using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PaymentMethods;
using Application.Requests.PaymentMethods;
using Domain.Entities.Invoices;
using Domain.ValueObject.Invoices.PaymentMethod;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly AutoTallerDbContext _dbContext;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, AutoTallerDbContext dbContext)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
    {
        var methods = await _paymentMethodRepository.GetAllAsync();
        return methods.Select(MapToDto);
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(int id)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(id);
        return method is null ? null : MapToDto(method);
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodRequest request)
    {
        await EnsureNameIsUniqueAsync(request.Name);

        var method = new PaymentMethod(new PaymentMethodName(request.Name));
        await _paymentMethodRepository.AddAsync(method);
        await _dbContext.SaveChangesAsync();

        return MapToDto(method);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePaymentMethodRequest request)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(id);
        if (method is null)
        {
            return false;
        }

        await EnsureNameIsUniqueAsync(request.Name, id);

        method.Update(new PaymentMethodName(request.Name));
        _paymentMethodRepository.Update(method);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(id);
        if (method is null)
        {
            return false;
        }

        if (await _dbContext.Payments.AnyAsync(x => x.PaymentMethodId == id))
        {
            throw new InvalidOperationException($"Payment method {id} is being used and cannot be deleted.");
        }

        _paymentMethodRepository.Remove(method);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await _dbContext.PaymentMethods.AnyAsync(x =>
            x.Name.Value.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"Payment method '{name}' already exists.");
        }
    }

    private static PaymentMethodDto MapToDto(PaymentMethod method)
    {
        return new PaymentMethodDto
        {
            Id = method.Id,
            Name = method.Name.Value
        };
    }
}

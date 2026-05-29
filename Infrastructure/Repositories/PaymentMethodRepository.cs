using Application.Contracts.Repositories;
using Domain.Entities.Invoices;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PaymentMethodRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentMethod?> GetByIdAsync(int id)
    {
        return await _dbContext.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
    {
        return await _dbContext.PaymentMethods
            .OrderBy(x => x.Name.Value)
            .ToListAsync();
    }

    public async Task AddAsync(PaymentMethod paymentMethod)
    {
        await _dbContext.PaymentMethods.AddAsync(paymentMethod);
    }

    public void Update(PaymentMethod paymentMethod)
    {
        _dbContext.PaymentMethods.Update(paymentMethod);
    }

    public void Remove(PaymentMethod paymentMethod)
    {
        _dbContext.PaymentMethods.Remove(paymentMethod);
    }
}

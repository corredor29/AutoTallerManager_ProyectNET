using Application.Contracts.Repositories;
using Domain.Entities.Invoices;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public PaymentRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _dbContext.Payments
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _dbContext.Payments
            .Include(x => x.PaymentMethod)
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
    }

    public void Update(Payment payment)
    {
        _dbContext.Payments.Update(payment);
    }

    public void Remove(Payment payment)
    {
        _dbContext.Payments.Remove(payment);
    }
}

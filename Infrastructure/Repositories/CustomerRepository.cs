using Application.Contracts.Repositories;
using Domain.Entities.Customers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly AutoTallerDbContext _dbContext;

        public CustomerRepository(AutoTallerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _dbContext.Customers
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.Person)
                .ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
        }

        public void Update(Customer customer)
        {
            _dbContext.Customers.Update(customer);
        }

        public void Remove(Customer customer)
        {
            _dbContext.Customers.Remove(customer);
        }
    }
}

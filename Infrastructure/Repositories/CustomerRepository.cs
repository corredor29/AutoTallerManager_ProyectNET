using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
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

        public async Task<PagedResult<Customer>> GetAllPagedAsync(
            PaginationParams pagination, CustomerFilter filter)
        {
            var allCustomers = await _dbContext.Customers
                .Include(c => c.Person)
                    .ThenInclude(p => p.Documents)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Emails)
                        .ThenInclude(e => e.EmailDomain)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Phones)
                .ToListAsync();

            var query = allCustomers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
                query = query.Where(c => c.Person.FirstName.Value
                    .Contains(filter.FirstName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.LastName))
                query = query.Where(c => c.Person.LastName.Value
                    .Contains(filter.LastName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.DocumentNumber))
                query = query.Where(c => c.Person.Documents.Any(d =>
                    d.DocumentNumber.Value.Contains(filter.DocumentNumber.Trim(), StringComparison.OrdinalIgnoreCase)));

            if (filter.IsActive.HasValue)
                query = query.Where(c => c.Status.Value == filter.IsActive.Value);

            var totalCount = query.Count();
            var page       = pagination.NormalizedPageNumber;
            var size       = pagination.NormalizedPageSize;

            var items = query
                .OrderBy(c => c.Person.LastName.Value)
                .ThenBy(c => c.Person.FirstName.Value)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new PagedResult<Customer>
            {
                Items      = items,
                PageNumber = page,
                PageSize   = size,
                TotalCount = totalCount
            };
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _dbContext.Customers
                .Include(c => c.Person)
                    .ThenInclude(p => p.Documents)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Emails)
                        .ThenInclude(e => e.EmailDomain)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Phones)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.Person)
                    .ThenInclude(p => p.Documents)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Emails)
                        .ThenInclude(e => e.EmailDomain)
                .Include(c => c.Person)
                    .ThenInclude(p => p.Phones)
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
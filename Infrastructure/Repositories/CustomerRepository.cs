using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Customers;
using Infrastructure.Context;
using Infrastructure.Extensions;
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
            var query = _dbContext.Customers
                .Include(c => c.Person)
                    .ThenInclude(p => p.Documents)
                .AsQueryable();

            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(filter.FirstName),
                    c => EF.Functions.ILike(c.Person.FirstName.Value, "%" + filter.FirstName!.Trim() + "%"))
                .WhereIf(!string.IsNullOrWhiteSpace(filter.LastName),
                    c => EF.Functions.ILike(c.Person.LastName.Value, "%" + filter.LastName!.Trim() + "%"))
                .WhereIf(!string.IsNullOrWhiteSpace(filter.DocumentNumber),
                    c => c.Person.Documents.Any(d =>
                        EF.Functions.ILike(d.DocumentNumber.Value, "%" + filter.DocumentNumber!.Trim() + "%")))
                .WhereIf(filter.IsActive.HasValue, c => c.Status.Value == filter.IsActive!.Value);

            var totalCount = await query.CountAsync();
            var page = pagination.NormalizedPageNumber;
            var size = pagination.NormalizedPageSize;

            var items = await query
                .OrderBy(c => c.Person.LastName.Value)
                .ThenBy(c => c.Person.FirstName.Value)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Items     = items,
                PageNumber = page,
                PageSize   = size,
                TotalCount = totalCount
            };
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

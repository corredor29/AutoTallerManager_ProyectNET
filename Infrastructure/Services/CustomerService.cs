using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.Requests.Customers;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.Person;
using Infrastructure.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly AutoTallerDbContext _dbContext;

        public CustomerService(ICustomerRepository customerRepository, AutoTallerDbContext dbContext)
        {
            _customerRepository = customerRepository;
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CustomerDto>> GetAllAsync(GetCustomersRequest request)
        {
            var query = _dbContext.Customers
                .Include(x => x.Person)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";
                query = query.Where(x =>
                    EF.Functions.ILike(x.Person.FirstName.Value, pattern) ||
                    EF.Functions.ILike(x.Person.LastName.Value, pattern));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.Status.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync();
            var pageNumber = request.NormalizedPageNumber;
            var pageSize = request.NormalizedPageSize;

            var customers = await query
                .OrderBy(x => x.Person.FirstName.Value)
                .ThenBy(x => x.Person.LastName.Value)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CustomerDto>
            {
                Items = customers.Select(x => x.Adapt<CustomerDto>()).ToArray(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer?.Adapt<CustomerDto>();
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
        {
            var person = new Person(new PersonFirstName(request.FirstName), new PersonLastName(request.LastName));
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            var customer = new Customer(person.Id);
            await _customerRepository.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Entry(customer).Reference(c => c.Person).LoadAsync();
            return customer.Adapt<CustomerDto>();
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
            {
                return false;
            }

            customer.Person.Update(new PersonFirstName(request.FirstName), new PersonLastName(request.LastName));
            if (request.IsActive)
            {
                customer.Activate();
            }
            else
            {
                customer.Deactivate();
            }

            _customerRepository.Update(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
            {
                return false;
            }

            _customerRepository.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

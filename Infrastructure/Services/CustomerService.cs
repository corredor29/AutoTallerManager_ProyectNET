using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.Requests.Customers;
using Application.Mapping;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.Person;
using Infrastructure.Context;
using Mapster;

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

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(x => x.Adapt<CustomerDto>());
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

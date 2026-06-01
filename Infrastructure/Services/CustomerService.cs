using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.DTOs.Persons;
using Application.Filters;
using Application.Requests.Customers;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.Person;
using Infrastructure.Context;
using Mapster;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.PersonEmail;
using Domain.ValueObject.Persons.PersonPhone;
using Domain.ValueObject.Persons.PersonDocument;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly AutoTallerDbContext  _dbContext;

        public CustomerService(ICustomerRepository customerRepository, AutoTallerDbContext dbContext)
        {
            _customerRepository = customerRepository;
            _dbContext          = dbContext;
        }

        public async Task<PagedResult<CustomerDto>> GetAllPagedAsync(
            PaginationParams pagination, CustomerFilter filter)
        {
            var result = await _customerRepository.GetAllPagedAsync(pagination, filter);
            return new PagedResult<CustomerDto>
            {
                Items      = result.Items.Select(MapToDto).ToArray(),
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer is null ? null : MapToDto(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
        {
            var person = new Person(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName));

            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            // Guardar email
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var parts = request.Email.Trim().ToLowerInvariant().Split('@');
                if (parts.Length == 2)
                {
                    var allDomains = await _dbContext.EmailDomains.ToListAsync();
                    var domain = allDomains.FirstOrDefault(d => d.Domain.Value == parts[1]);
                    if (domain is null)
                    {
                        domain = new EmailDomain(new EmailDomainValue(parts[1]));
                        await _dbContext.EmailDomains.AddAsync(domain);
                        await _dbContext.SaveChangesAsync();
                    }
                    var personEmail = new PersonEmail(person.Id, domain.Id, new EmailUser(parts[0]), true);
                    await _dbContext.PersonEmails.AddAsync(personEmail);
                }
            }

            // Guardar teléfono
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var allCodes = await _dbContext.PhoneCodes.ToListAsync();
                var phoneCode = allCodes.FirstOrDefault();
                if (phoneCode is not null)
                {
                    var personPhone = new PersonPhone(person.Id, phoneCode.Id, new PhoneNumber(request.Phone), true);
                    await _dbContext.PersonPhones.AddAsync(personPhone);
                }
            }

            // Guardar documento
            if (request.DocumentTypeId.HasValue && !string.IsNullOrWhiteSpace(request.DocumentNumber))
            {
                var personDoc = new PersonDocument(
                    person.Id,
                    request.DocumentTypeId.Value,
                    new DocumentNumber(request.DocumentNumber));
                await _dbContext.PersonDocuments.AddAsync(personDoc);
            }

            await _dbContext.SaveChangesAsync();

            var customer = new Customer(person.Id);
            await _customerRepository.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(customer.Id) ?? MapToDto(customer);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null) return false;

            customer.Person.Update(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName));

            if (request.IsActive)
                customer.Activate();
            else
                customer.Deactivate();

            _customerRepository.Update(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null) return false;

            await EnsureCustomerCanBeDeletedAsync(id);

            _customerRepository.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task EnsureCustomerCanBeDeletedAsync(int customerId)
        {
            var hasAppointments = await _dbContext.Appointments
                .AnyAsync(x => x.CustomerId == customerId);

            if (hasAppointments)
                throw new InvalidOperationException(
                    $"Customer {customerId} cannot be deleted because it has appointments associated.");

            var hasServiceOrders = await _dbContext.ServiceOrders.AnyAsync(x =>
                x.AppointmentId.HasValue &&
                _dbContext.Appointments.Any(a =>
                    a.Id == x.AppointmentId.Value &&
                    a.CustomerId == customerId));

            if (hasServiceOrders)
                throw new InvalidOperationException(
                    $"Customer {customerId} cannot be deleted because it has service orders associated.");
        }

        private static CustomerDto MapToDto(Customer c) => new()
        {
            Id               = c.Id,
            PersonId         = c.PersonId,
            IsActive         = c.Status?.Value ?? true,
            FirstName        = c.Person?.FirstName?.Value ?? string.Empty,
            LastName         = c.Person?.LastName?.Value  ?? string.Empty,
            PrimaryEmail = c.Person?.Emails?
                .FirstOrDefault(e => e.IsPrimary) is { } primaryEmail
                ? $"{primaryEmail.EmailUser.Value}@{primaryEmail.EmailDomain?.Domain?.Value}"
                : "—",
            PrimaryPhone     = c.Person?.Phones?
                                 .FirstOrDefault(p => p.IsPrimary)?.PhoneNumber?.Value ?? "—",
            PrimaryDocument  = c.Person?.Documents?
                                 .FirstOrDefault()?.DocumentNumber?.Value ?? "—",
            Person           = c.Person?.Adapt<PersonDto>()!
        };
    }
}
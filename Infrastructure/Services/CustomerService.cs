using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.DTOs.Persons;
using Application.Filters;
using Application.Requests.Customers;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Persons.PersonDocument;
using Domain.ValueObject.Persons.PersonEmail;
using Domain.ValueObject.Persons.PersonPhone;
using Domain.ValueObject.Vehicles.Vehicle;
using Infrastructure.Context;
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

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var parts = request.Email.Trim().ToLowerInvariant().Split('@');
                if (parts.Length == 2)
                {
                    var domain = await GetOrCreateEmailDomainAsync(parts[1]);
                    await _dbContext.PersonEmails.AddAsync(
                        new PersonEmail(person.Id, domain.Id, new EmailUser(parts[0]), true));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var allCodes  = await _dbContext.PhoneCodes.ToListAsync();
                var phoneCode = allCodes.FirstOrDefault();
                if (phoneCode is not null)
                    await _dbContext.PersonPhones.AddAsync(
                        new PersonPhone(person.Id, phoneCode.Id, new PhoneNumber(request.Phone), true));
            }

            if (request.DocumentTypeId.HasValue && !string.IsNullOrWhiteSpace(request.DocumentNumber))
                await _dbContext.PersonDocuments.AddAsync(
                    new PersonDocument(person.Id, request.DocumentTypeId.Value,
                        new DocumentNumber(request.DocumentNumber)));

            await _dbContext.SaveChangesAsync();

            var customer = new Customer(person.Id);
            await _customerRepository.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(customer.Id) ?? MapToDto(customer);
        }

        public async Task<CustomerRegistrationDto> RegisterWithVehicleAsync(
            RegisterCustomerWithVehicleRequest request)
        {
            await using var transaction = _dbContext.Database.IsRelational()
                ? await _dbContext.Database.BeginTransactionAsync()
                : null;

            try
            {
                var person = new Person(
                    new PersonFirstName(request.FirstName),
                    new PersonLastName(request.LastName));
                await _dbContext.Persons.AddAsync(person);
                await _dbContext.SaveChangesAsync();

                var (emailUser, emailDomainValue) = ParseEmail(request.Email);
                var emailDomain = await GetOrCreateEmailDomainAsync(emailDomainValue);
                await _dbContext.PersonEmails.AddAsync(
                    new PersonEmail(person.Id, emailDomain.Id, new EmailUser(emailUser), true));

                if (!await _dbContext.PhoneCodes.AnyAsync(x => x.Id == request.PhoneCodeId))
                    throw new ArgumentException($"Phone code {request.PhoneCodeId} does not exist.");

                await _dbContext.PersonPhones.AddAsync(
                    new PersonPhone(person.Id, request.PhoneCodeId,
                        new PhoneNumber(request.PhoneNumber), true));

                await _dbContext.SaveChangesAsync();

                var customer = new Customer(person.Id);
                await _customerRepository.AddAsync(customer);
                await _dbContext.SaveChangesAsync();

                var vehicle = await CreateVehicleAsync(request.Vehicle);

                await _dbContext.VehicleOwnershipHistories.AddAsync(
                    new VehicleOwnershipHistory(
                        vehicle.Id,
                        customer.Id,
                        new Domain.ValueObject.Vehicles.VehicleOwnershipHistory.DateRange(
                            DateOnly.FromDateTime(DateTime.UtcNow), null)));

                await _dbContext.SaveChangesAsync();

                if (transaction is not null)
                    await transaction.CommitAsync();

                var createdCustomer = await _customerRepository.GetByIdAsync(customer.Id)
                    ?? throw new InvalidOperationException("Customer could not be reloaded after registration.");

                await _dbContext.Entry(vehicle).Reference(v => v.Model).LoadAsync();
                await _dbContext.Entry(vehicle.Model).Reference(m => m.Brand).LoadAsync();
                await _dbContext.Entry(vehicle).Reference(v => v.Color).LoadAsync();
                await _dbContext.Entry(vehicle).Reference(v => v.FuelType).LoadAsync();
                await _dbContext.Entry(vehicle).Reference(v => v.TransmissionType).LoadAsync();

                return new CustomerRegistrationDto
                {
                    Customer = MapToDto(createdCustomer),
                    Vehicle  = MapVehicleToDto(vehicle)
                };
            }
            catch
            {
                if (transaction is not null)
                    await transaction.RollbackAsync();
                throw;
            }
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

        // ── Helpers ────────────────────────────────────
        private static (string emailUser, string emailDomain) ParseEmail(string email)
        {
            var parts = email.Trim().ToLowerInvariant().Split('@');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid email format.");
            return (parts[0], parts[1]);
        }

        private async Task<EmailDomain> GetOrCreateEmailDomainAsync(string domainValue)
        {
            var allDomains = await _dbContext.EmailDomains.ToListAsync();
            var domain     = allDomains.FirstOrDefault(d => d.Domain.Value == domainValue);
            if (domain is null)
            {
                domain = new EmailDomain(new EmailDomainValue(domainValue));
                await _dbContext.EmailDomains.AddAsync(domain);
                await _dbContext.SaveChangesAsync();
            }
            return domain;
        }

        private async Task<Vehicle> CreateVehicleAsync(RegisterVehicleRequest request)
        {
            if (!await _dbContext.VehicleModels.AnyAsync(m => m.Id == request.ModelId))
                throw new ArgumentException($"Vehicle model {request.ModelId} does not exist.");

            var existing = await _dbContext.Vehicles.ToListAsync();
            if (existing.Any(v => v.VIN.Value.ToUpperInvariant() == request.Vin.Trim().ToUpperInvariant()))
                throw new InvalidOperationException($"Vehicle VIN '{request.Vin}' is already registered.");

            var vehicle = new Vehicle(
                request.ModelId,
                new VinNumber(request.Vin),
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate)
                    ? null
                    : new LicensePlate(request.LicensePlate));

            await _dbContext.Vehicles.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle;
        }

        private static Application.DTOs.Vehicles.VehicleDto MapVehicleToDto(Vehicle v) => new()
        {
            Id                   = v.Id,
            ModelId              = v.ModelId,
            ColorId              = v.ColorId,
            FuelTypeId           = v.FuelTypeId,
            TransmissionTypeId   = v.TransmissionTypeId,
            Vin                  = v.VIN.Value,
            Year                 = v.Year.Value,
            Mileage              = v.Mileage.Value,
            LicensePlate         = v.LicensePlate?.Value,
            ModelName            = v.Model?.ModelName.Value        ?? string.Empty,
            BrandName            = v.Model?.Brand?.BrandName.Value ?? string.Empty,
            ColorName            = v.Color?.Name.Value,
            FuelTypeName         = v.FuelType?.Name.Value,
            TransmissionTypeName = v.TransmissionType?.Name.Value,
            CurrentOwnerName     = null
        };

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
                    a.Id == x.AppointmentId.Value && a.CustomerId == customerId));
            if (hasServiceOrders)
                throw new InvalidOperationException(
                    $"Customer {customerId} cannot be deleted because it has service orders associated.");
        }

        private static string GetPrimaryEmail(Customer c)
        {
            try
            {
                var primary = c.Person?.Emails?.FirstOrDefault(e => e.IsPrimary);
                if (primary is null) return "—";
                var user   = primary.EmailUser?.Value          ?? string.Empty;
                var domain = primary.EmailDomain?.Domain?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(user)) return "—";
                if (string.IsNullOrEmpty(domain)) return user;
                return $"{user}@{domain}";
            }
            catch { return "—"; }
        }

        private static CustomerDto MapToDto(Customer c) => new()
        {
            Id              = c.Id,
            PersonId        = c.PersonId,
            IsActive        = c.Status?.Value ?? true,
            FirstName       = c.Person?.FirstName?.Value ?? string.Empty,
            LastName        = c.Person?.LastName?.Value  ?? string.Empty,
            PrimaryEmail    = GetPrimaryEmail(c),
            PrimaryPhone    = c.Person?.Phones?
                                .FirstOrDefault(p => p.IsPrimary)?.PhoneNumber?.Value ?? "—",
            PrimaryDocument = c.Person?.Documents?
                                .FirstOrDefault()?.DocumentNumber?.Value ?? "—",
            Person          = c.Person is null ? null! : new PersonDto
            {
                Id           = c.Person.Id,
                FirstName    = c.Person.FirstName?.Value ?? string.Empty,
                LastName     = c.Person.LastName?.Value  ?? string.Empty,
                PrimaryEmail = GetPrimaryEmail(c),
                PrimaryPhone = c.Person.Phones?
                                 .FirstOrDefault(p => p.IsPrimary)?.PhoneNumber?.Value,
                RegisteredAt = c.Person.RegisteredAt
            }
        };
    }
}
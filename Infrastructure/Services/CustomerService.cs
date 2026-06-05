using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.DTOs.Notifications;
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
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository         _customerRepository;
        private readonly AutoTallerDbContext          _dbContext;
        private readonly IHubContext<NotificationHub> _hub;

        public CustomerService(
            ICustomerRepository customerRepository,
            AutoTallerDbContext dbContext,
            IHubContext<NotificationHub> hub)
        {
            _customerRepository = customerRepository;
            _dbContext          = dbContext;
            _hub                = hub;
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
            var normalizedEmail          = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            var normalizedPhone          = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            var normalizedDocumentNumber = string.IsNullOrWhiteSpace(request.DocumentNumber) ? null : request.DocumentNumber.Trim();

            if (request.DocumentTypeId.HasValue != (normalizedDocumentNumber is not null))
                throw new ArgumentException("Document type and document number must be provided together.");

            string? emailUser = null;
            string? emailDomainValue = null;

            if (normalizedEmail is not null)
            {
                (emailUser, emailDomainValue) = ParseEmail(normalizedEmail);
                await EnsureEmailIsAvailableAsync(emailUser, emailDomainValue);
            }

            int? phoneCodeId = null;
            if (normalizedPhone is not null)
            {
                phoneCodeId = await GetDefaultPhoneCodeIdAsync();
                await EnsurePhoneIsAvailableAsync(phoneCodeId.Value, normalizedPhone);
            }

            if (request.DocumentTypeId.HasValue && normalizedDocumentNumber is not null)
            {
                await EnsureDocumentTypeExistsAsync(request.DocumentTypeId.Value);
                await EnsureDocumentIsAvailableAsync(request.DocumentTypeId.Value, normalizedDocumentNumber);
            }

            var person = new Person(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName));

            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            if (emailUser is not null && emailDomainValue is not null)
            {
                var domain = await GetOrCreateEmailDomainAsync(emailDomainValue);
                await _dbContext.PersonEmails.AddAsync(
                    new PersonEmail(person.Id, domain.Id, new EmailUser(emailUser), true));
            }

            if (normalizedPhone is not null && phoneCodeId.HasValue)
            {
                await _dbContext.PersonPhones.AddAsync(
                    new PersonPhone(person.Id, phoneCodeId.Value, new PhoneNumber(normalizedPhone), true));
            }

            if (request.DocumentTypeId.HasValue && normalizedDocumentNumber is not null)
                await _dbContext.PersonDocuments.AddAsync(
                    new PersonDocument(person.Id, request.DocumentTypeId.Value,
                        new DocumentNumber(normalizedDocumentNumber)));

            await _dbContext.SaveChangesAsync();

            var customer = new Customer(person.Id);
            await _customerRepository.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "create",
                Entity     = "Customer",
                RecordId   = customer.Id,
                Message    = $"New customer {request.FirstName} {request.LastName} registered",
                OccurredAt = DateTime.UtcNow
            });

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
                var (emailUser, emailDomainValue) = ParseEmail(request.Email);

                await EnsureEmailIsAvailableAsync(emailUser, emailDomainValue);

                if (!await _dbContext.PhoneCodes.AnyAsync(x => x.Id == request.PhoneCodeId))
                    throw new ArgumentException($"Phone code {request.PhoneCodeId} does not exist.");

                await EnsurePhoneIsAvailableAsync(request.PhoneCodeId, request.PhoneNumber.Trim());

                var person = new Person(
                    new PersonFirstName(request.FirstName),
                    new PersonLastName(request.LastName));
                await _dbContext.Persons.AddAsync(person);
                await _dbContext.SaveChangesAsync();

                var emailDomain = await GetOrCreateEmailDomainAsync(emailDomainValue);
                await _dbContext.PersonEmails.AddAsync(
                    new PersonEmail(person.Id, emailDomain.Id, new EmailUser(emailUser), true));

                await _dbContext.PersonPhones.AddAsync(
                    new PersonPhone(person.Id, request.PhoneCodeId,
                        new PhoneNumber(request.PhoneNumber.Trim()), true));

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

                await _hub.Clients.All.SendAsync("Notification", new NotificationDto
                {
                    Type       = "create",
                    Entity     = "Customer",
                    RecordId   = customer.Id,
                    Message    = $"New customer {request.FirstName} {request.LastName} registered with vehicle",
                    OccurredAt = DateTime.UtcNow
                });

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

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "update",
                Entity     = "Customer",
                RecordId   = id,
                Message    = $"Customer {request.FirstName} {request.LastName} updated",
                OccurredAt = DateTime.UtcNow
            });

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null) return false;

            await EnsureCustomerCanBeDeletedAsync(id);
            _customerRepository.Remove(customer);
            await _dbContext.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("Notification", new NotificationDto
            {
                Type       = "delete",
                Entity     = "Customer",
                RecordId   = id,
                Message    = $"Customer #{id} deleted",
                OccurredAt = DateTime.UtcNow
            });

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

        private async Task EnsureEmailIsAvailableAsync(string emailUser, string emailDomain)
        {
            var allEmails = await _dbContext.PersonEmails
                .Include(x => x.EmailDomain)
                .ToListAsync();

            var emailExists = allEmails.Any(x =>
                x.EmailUser.Value.Equals(emailUser, StringComparison.OrdinalIgnoreCase) &&
                x.EmailDomain.Domain.Value.Equals(emailDomain, StringComparison.OrdinalIgnoreCase));

            if (emailExists)
                throw new InvalidOperationException($"Email '{emailUser}@{emailDomain}' is already registered.");
        }

        private async Task<int> GetDefaultPhoneCodeIdAsync()
        {
            var phoneCode = await _dbContext.PhoneCodes
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            return phoneCode?.Id
                ?? throw new InvalidOperationException("No phone codes are configured for customer registration.");
        }

        private async Task EnsurePhoneIsAvailableAsync(int phoneCodeId, string phoneNumber)
        {
            var normalizedPhone = phoneNumber.Trim();
            var allPhones = await _dbContext.PersonPhones.ToListAsync();

            var phoneExists = allPhones.Any(x =>
                x.PhoneCodeId == phoneCodeId &&
                x.PhoneNumber.Value.Equals(normalizedPhone, StringComparison.OrdinalIgnoreCase));

            if (phoneExists)
                throw new InvalidOperationException($"Phone '{normalizedPhone}' is already registered.");
        }

        private async Task EnsureDocumentTypeExistsAsync(int documentTypeId)
        {
            if (!await _dbContext.DocumentTypes.AnyAsync(x => x.Id == documentTypeId))
                throw new ArgumentException($"Document type {documentTypeId} does not exist.");
        }

        private async Task EnsureDocumentIsAvailableAsync(int documentTypeId, string documentNumber)
        {
            var normalizedDocumentNumber = documentNumber.Trim();
            var allDocuments = await _dbContext.PersonDocuments.ToListAsync();

            var documentExists = allDocuments.Any(x =>
                x.DocumentTypeId == documentTypeId &&
                x.DocumentNumber.Value.Equals(normalizedDocumentNumber, StringComparison.OrdinalIgnoreCase));

            if (documentExists)
                throw new InvalidOperationException($"Document '{normalizedDocumentNumber}' is already registered.");
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
                var user   = primary.EmailUser?.Value           ?? string.Empty;
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

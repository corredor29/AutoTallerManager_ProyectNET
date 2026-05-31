using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Customers;
using Application.DTOs.Vehicles;
using Application.Filters;
using Application.Requests.Customers;
using Domain.Entities.Customers;
using Domain.Entities.Persons;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Persons.PersonEmail;
using Domain.ValueObject.Persons.PersonPhone;
using Domain.ValueObject.Vehicles.Vehicle;
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

        public async Task<PagedResult<CustomerDto>> GetAllPagedAsync(
            PaginationParams pagination, CustomerFilter filter)
        {
            var result = await _customerRepository.GetAllPagedAsync(pagination, filter);
            return new PagedResult<CustomerDto>
            {
                Items      = result.Items.Select(c => c.Adapt<CustomerDto>()).ToArray(),
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize,
                TotalCount = result.TotalCount
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

        public async Task<CustomerRegistrationDto> RegisterWithVehicleAsync(RegisterCustomerWithVehicleRequest request)
        {
            await using var transaction = _dbContext.Database.IsRelational()
                ? await _dbContext.Database.BeginTransactionAsync()
                : null;

            var person = new Person(new PersonFirstName(request.FirstName), new PersonLastName(request.LastName));
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            var (emailUser, emailDomainValue) = ParseEmail(request.Email);
            var emailDomain = await GetOrCreateEmailDomainAsync(emailDomainValue);

            if (!await _dbContext.PhoneCodes.AnyAsync(x => x.Id == request.PhoneCodeId))
            {
                throw new ArgumentException($"Phone code {request.PhoneCodeId} does not exist.");
            }

            await _dbContext.PersonEmails.AddAsync(new PersonEmail(
                person.Id,
                emailDomain.Id,
                new EmailUser(emailUser),
                true));

            await _dbContext.PersonPhones.AddAsync(new PersonPhone(
                person.Id,
                request.PhoneCodeId,
                new PhoneNumber(request.PhoneNumber),
                true));

            var customer = new Customer(person.Id);
            await _customerRepository.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            var vehicle = await CreateVehicleAsync(request.Vehicle);

            await _dbContext.VehicleOwnershipHistories.AddAsync(new VehicleOwnershipHistory(
                vehicle.Id,
                customer.Id,
                new Domain.ValueObject.Vehicles.VehicleOwnershipHistory.DateRange(
                    DateOnly.FromDateTime(DateTime.UtcNow))));

            await _dbContext.SaveChangesAsync();
            if (transaction is not null)
            {
                await transaction.CommitAsync();
            }

            var createdCustomer = await _customerRepository.GetByIdAsync(customer.Id)
                ?? throw new InvalidOperationException("Customer could not be reloaded after registration.");

            await _dbContext.Entry(vehicle).Reference(v => v.Model).LoadAsync();
            await _dbContext.Entry(vehicle.Model).Reference(m => m.Brand).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.Color).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.FuelType).LoadAsync();
            await _dbContext.Entry(vehicle).Reference(v => v.TransmissionType).LoadAsync();

            return new CustomerRegistrationDto
            {
                Customer = createdCustomer.Adapt<CustomerDto>(),
                Vehicle = MapVehicleToDto(vehicle)
            };
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

            await EnsureCustomerCanBeDeletedAsync(id);

            _customerRepository.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task EnsureCustomerCanBeDeletedAsync(int customerId)
        {
            var hasAppointments = await _dbContext.Appointments.AnyAsync(x => x.CustomerId == customerId);
            if (hasAppointments)
            {
                throw new InvalidOperationException(
                    $"Customer {customerId} cannot be deleted because it has appointments associated.");
            }

            var hasServiceOrders = await _dbContext.ServiceOrders.AnyAsync(x =>
                x.AppointmentId.HasValue &&
                _dbContext.Appointments.Any(a => a.Id == x.AppointmentId.Value && a.CustomerId == customerId));

            if (hasServiceOrders)
            {
                throw new InvalidOperationException(
                    $"Customer {customerId} cannot be deleted because it has service orders associated.");
            }
        }

        private async Task<EmailDomain> GetOrCreateEmailDomainAsync(string domainValue)
        {
            var normalizedDomain = new EmailDomainValue(domainValue);
            var emailDomain = await _dbContext.EmailDomains
                .FirstOrDefaultAsync(x => x.Domain == normalizedDomain);

            if (emailDomain is not null)
            {
                return emailDomain;
            }

            emailDomain = new EmailDomain(normalizedDomain);
            await _dbContext.EmailDomains.AddAsync(emailDomain);
            await _dbContext.SaveChangesAsync();
            return emailDomain;
        }

        private async Task<Vehicle> CreateVehicleAsync(RegisterVehicleRequest request)
        {
            if (!await _dbContext.VehicleModels.AnyAsync(x => x.Id == request.ModelId))
            {
                throw new ArgumentException($"Vehicle model {request.ModelId} does not exist.");
            }

            if (request.ColorId.HasValue && !await _dbContext.VehicleColors.AnyAsync(x => x.Id == request.ColorId.Value))
            {
                throw new ArgumentException($"Vehicle color {request.ColorId.Value} does not exist.");
            }

            if (request.FuelTypeId.HasValue && !await _dbContext.FuelTypes.AnyAsync(x => x.Id == request.FuelTypeId.Value))
            {
                throw new ArgumentException($"Fuel type {request.FuelTypeId.Value} does not exist.");
            }

            if (request.TransmissionTypeId.HasValue &&
                !await _dbContext.TransmissionTypes.AnyAsync(x => x.Id == request.TransmissionTypeId.Value))
            {
                throw new ArgumentException($"Transmission type {request.TransmissionTypeId.Value} does not exist.");
            }

            var normalizedVin = request.Vin.Trim().ToUpperInvariant();
            if (await _dbContext.Vehicles.AnyAsync(x => x.VIN.Value == normalizedVin))
            {
                throw new InvalidOperationException($"Vehicle VIN '{request.Vin}' is already registered.");
            }

            var vehicle = new Vehicle(
                request.ModelId,
                new VinNumber(normalizedVin),
                new VehicleYear(request.Year),
                new VehicleMileage(request.Mileage),
                request.ColorId,
                request.FuelTypeId,
                request.TransmissionTypeId,
                string.IsNullOrWhiteSpace(request.LicensePlate) ? null : new LicensePlate(request.LicensePlate));

            await _dbContext.Vehicles.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle;
        }

        private static (string User, string Domain) ParseEmail(string email)
        {
            var parts = email
                .Trim()
                .ToLowerInvariant()
                .Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
            {
                throw new ArgumentException("Email format is invalid.");
            }

            return (parts[0], parts[1]);
        }

        private static VehicleDto MapVehicleToDto(Vehicle vehicle)
        {
            return new VehicleDto
            {
                Id = vehicle.Id,
                ModelId = vehicle.ModelId,
                ColorId = vehicle.ColorId,
                FuelTypeId = vehicle.FuelTypeId,
                TransmissionTypeId = vehicle.TransmissionTypeId,
                Vin = vehicle.VIN.Value,
                Year = vehicle.Year.Value,
                Mileage = vehicle.Mileage.Value,
                LicensePlate = vehicle.LicensePlate?.Value,
                ModelName = vehicle.Model?.ModelName.Value ?? string.Empty,
                BrandName = vehicle.Model?.Brand?.BrandName.Value ?? string.Empty,
                ColorName = vehicle.Color?.Name.Value,
                FuelTypeName = vehicle.FuelType?.Name.Value,
                TransmissionTypeName = vehicle.TransmissionType?.Name.Value
            };
        }
    }
}

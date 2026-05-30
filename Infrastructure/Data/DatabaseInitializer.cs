using Domain.Entities.ServiceOrders;
using Domain.Entities.Quotations;
using Domain.Entities.Suppliers;
using Domain.Entities.Invoices;
using Domain.Entities.Appointments;
using Domain.Entities.Users;
using Domain.Entities.Persons;
using Domain.ValueObject.ServiceOrders.OrderStatus;
using Domain.ValueObject.ServiceOrders.ServiceType;
using Domain.ValueObject.Quotations.QuotationStatus;
using Domain.ValueObject.Suppliers.PurchaseOrderStatus;
using Domain.ValueObject.Invoices.PaymentMethod;
using Domain.ValueObject.Appointments.AppointmentStatus;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Users.Role;
using Domain.ValueObject.Users.User;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class DatabaseInitializer
{
    private readonly AutoTallerDbContext _dbContext;

    public DatabaseInitializer(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.MigrateAsync();

        await SeedAppointmentStatusesAsync();
        await SeedOrderStatusesAsync();
        await SeedServiceTypesAsync();
        await SeedQuotationStatusesAsync();
        await SeedPurchaseOrderStatusesAsync();
        await SeedPaymentMethodsAsync();
        await SeedRolesAsync();
        await SeedDefaultAdminAsync();
    }

    private async Task SeedAppointmentStatusesAsync()
    {
        if (await _dbContext.AppointmentStatuses.AnyAsync())
        {
            return;
        }

        var statuses = new[]
        {
            new AppointmentStatus(new AppointmentStatusName("Pending")),
            new AppointmentStatus(new AppointmentStatusName("Confirmed")),
            new AppointmentStatus(new AppointmentStatusName("Completed")),
            new AppointmentStatus(new AppointmentStatusName("Cancelled"))
        };

        await _dbContext.AppointmentStatuses.AddRangeAsync(statuses);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedOrderStatusesAsync()
    {
        if (await _dbContext.OrderStatuses.AnyAsync())
        {
            return;
        }

        var statuses = new[]
        {
            new OrderStatus(new OrderStatusName("Pending")),
            new OrderStatus(new OrderStatusName("In Progress")),
            new OrderStatus(new OrderStatusName("Completed")),
            new OrderStatus(new OrderStatusName("Cancelled"))
        };

        await _dbContext.OrderStatuses.AddRangeAsync(statuses);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedServiceTypesAsync()
    {
        if (await _dbContext.ServiceTypes.AnyAsync())
        {
            return;
        }

        var serviceTypes = new[]
        {
            new ServiceType(new ServiceTypeName("Preventive Maintenance"), new ServiceDuration(4)),
            new ServiceType(new ServiceTypeName("Repair"), new ServiceDuration(8)),
            new ServiceType(new ServiceTypeName("Diagnostics"), new ServiceDuration(2))
        };

        await _dbContext.ServiceTypes.AddRangeAsync(serviceTypes);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedQuotationStatusesAsync()
    {
        if (await _dbContext.QuotationStatuses.AnyAsync())
        {
            return;
        }

        var quotationStatuses = new[]
        {
            new QuotationStatus(new QuotationStatusName("Pending")),
            new QuotationStatus(new QuotationStatusName("Accepted")),
            new QuotationStatus(new QuotationStatusName("Rejected"))
        };

        await _dbContext.QuotationStatuses.AddRangeAsync(quotationStatuses);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedPurchaseOrderStatusesAsync()
    {
        if (await _dbContext.PurchaseOrderStatuses.AnyAsync())
        {
            return;
        }

        var purchaseOrderStatuses = new[]
        {
            new PurchaseOrderStatus(new PurchaseOrderStatusName("Pending")),
            new PurchaseOrderStatus(new PurchaseOrderStatusName("Sent")),
            new PurchaseOrderStatus(new PurchaseOrderStatusName("Received")),
            new PurchaseOrderStatus(new PurchaseOrderStatusName("Cancelled"))
        };

        await _dbContext.PurchaseOrderStatuses.AddRangeAsync(purchaseOrderStatuses);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedPaymentMethodsAsync()
    {
        if (await _dbContext.PaymentMethods.AnyAsync())
        {
            return;
        }

        var paymentMethods = new[]
        {
            new PaymentMethod(new PaymentMethodName("Cash")),
            new PaymentMethod(new PaymentMethodName("Credit Card")),
            new PaymentMethod(new PaymentMethodName("Debit Card")),
            new PaymentMethod(new PaymentMethodName("Bank Transfer"))
        };

        await _dbContext.PaymentMethods.AddRangeAsync(paymentMethods);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await _dbContext.Roles.AnyAsync())
        {
            return;
        }

        var roles = new[]
        {
            new Role(new RoleName("Admin")),
            new Role(new RoleName("Mechanic")),
            new Role(new RoleName("Receptionist"))
        };

        await _dbContext.Roles.AddRangeAsync(roles);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedDefaultAdminAsync()
    {
        if (await _dbContext.Users.AnyAsync())
        {
            return;
        }

        var adminRole = (await _dbContext.Roles.ToListAsync())
            .FirstOrDefault(x => x.RoleName.Value == "Admin")
            ?? throw new InvalidOperationException("Admin role must exist before seeding the default admin user.");

        var person = new Person(new PersonFirstName("System"), new PersonLastName("Administrator"));
        await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var user = new User(person.Id, new PasswordHash(passwordHash));
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await _dbContext.UserRoles.AddAsync(new UserRole(user.Id, adminRole.Id));
        await _dbContext.SaveChangesAsync();
    }
}

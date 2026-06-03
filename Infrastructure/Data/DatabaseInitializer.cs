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
using Domain.ValueObject.Persons.PersonEmail;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Vehicles;
using Domain.Entities.Audit;
using Domain.ValueObject.Vehicles.VehicleBrand;
using Domain.ValueObject.Vehicles.VehicleColor;
using Domain.ValueObject.Vehicles.VehicleModel;
using Domain.ValueObject.Vehicles.FuelType;
using Domain.ValueObject.Vehicles.TransmissionType;
using Domain.ValueObject.Persons.DocumentType;
using Domain.ValueObject.Persons.EmailDomain;
using Domain.ValueObject.Persons.PhoneCode;
using Domain.ValueObject.Audit.AuditActionType;
using Domain.Entities.Parts;
using Domain.ValueObject.Parts.PartCategory;
using Domain.ValueObject.Parts.MeasurementUnit;

namespace Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly AutoTallerDbContext _dbContext;

    public DatabaseInitializer(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        if (_dbContext.Database.IsRelational())
            await _dbContext.Database.MigrateAsync();
        else
            await _dbContext.Database.EnsureCreatedAsync();

        await SeedAppointmentStatusesAsync();
        await SeedOrderStatusesAsync();
        await SeedServiceTypesAsync();
        await SeedQuotationStatusesAsync();
        await SeedPurchaseOrderStatusesAsync();
        await SeedPaymentMethodsAsync();
        await SeedRolesAsync();
        await SeedDefaultAdminAsync();
        await SeedDocumentTypesAsync();
        await SeedEmailDomainsAsync();
        await SeedPhoneCodesAsync();
        await SeedVehicleBrandsAsync();
        await SeedVehicleModelsAsync();
        await SeedVehicleColorsAsync();
        await SeedFuelTypesAsync();
        await SeedTransmissionTypesAsync();
        await SeedAuditActionTypesAsync();
        await SeedPartCategoriesAsync();
        await SeedMeasurementUnitsAsync();
    }

    private async Task SeedAppointmentStatusesAsync()
    {
        if (await _dbContext.AppointmentStatuses.AnyAsync()) return;
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
        if (await _dbContext.OrderStatuses.AnyAsync()) return;
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
        if (await _dbContext.ServiceTypes.AnyAsync()) return;
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
        if (await _dbContext.QuotationStatuses.AnyAsync()) return;
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
        if (await _dbContext.PurchaseOrderStatuses.AnyAsync()) return;
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
        if (await _dbContext.PaymentMethods.AnyAsync()) return;
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
        if (await _dbContext.Roles.AnyAsync()) return;
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
        var adminRole = (await _dbContext.Roles.ToListAsync())
            .FirstOrDefault(x => x.RoleName.Value == "Admin")
            ?? throw new InvalidOperationException("Admin role must exist before seeding the default admin user.");

        var adminUserId = await _dbContext.UserRoles
            .Where(x => x.RoleId == adminRole.Id)
            .Select(x => (int?)x.UserId)
            .FirstOrDefaultAsync();

        User adminUser;

        if (adminUserId.HasValue)
        {
            adminUser = await _dbContext.Users.FirstAsync(x => x.Id == adminUserId.Value);
        }
        else
        {
            var person = new Person(new PersonFirstName("System"), new PersonLastName("Administrator"));
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            adminUser = new User(person.Id, new PasswordHash(passwordHash));
            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.SaveChangesAsync();

            await _dbContext.UserRoles.AddAsync(new UserRole(adminUser.Id, adminRole.Id));
            await _dbContext.SaveChangesAsync();
        }

        await EnsureDefaultAdminEmailAsync(adminUser.PersonId);
    }

    private async Task EnsureDefaultAdminEmailAsync(int personId)
    {
        var hasPrimaryEmail = await _dbContext.PersonEmails.AnyAsync(x => x.PersonId == personId && x.IsPrimary);
        if (hasPrimaryEmail) return;

        var emailDomain = await _dbContext.EmailDomains
            .FirstOrDefaultAsync(x => x.Domain == new EmailDomainValue("autotaller.local"));

        if (emailDomain is null)
        {
            emailDomain = new EmailDomain(new EmailDomainValue("autotaller.local"));
            await _dbContext.EmailDomains.AddAsync(emailDomain);
            await _dbContext.SaveChangesAsync();
        }

        var existingAdminEmail = (await _dbContext.PersonEmails
            .Where(x => x.PersonId == personId && x.EmailDomainId == emailDomain.Id)
            .ToListAsync())
            .FirstOrDefault(x => x.EmailUser.Value == "admin");

        if (existingAdminEmail is null)
        {
            existingAdminEmail = new PersonEmail(personId, emailDomain.Id, new EmailUser("admin"), true);
            await _dbContext.PersonEmails.AddAsync(existingAdminEmail);
        }
        else
        {
            existingAdminEmail.SetAsPrimary();
            _dbContext.PersonEmails.Update(existingAdminEmail);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedDocumentTypesAsync()
    {
        if (await _dbContext.DocumentTypes.AnyAsync()) return;
        var documentTypes = new[]
        {
            new DocumentType(new DocumentCode("NID"), new DocumentTypeName("National Identity Document")),
            new DocumentType(new DocumentCode("TIN"), new DocumentTypeName("Tax Identification Number")),
            new DocumentType(new DocumentCode("FID"), new DocumentTypeName("Foreign Identity Document")),
            new DocumentType(new DocumentCode("PAS"), new DocumentTypeName("Passport"))
        };
        await _dbContext.DocumentTypes.AddRangeAsync(documentTypes);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedEmailDomainsAsync()
    {
        if (await _dbContext.EmailDomains.AnyAsync()) return;
        var domains = new[]
        {
            new EmailDomain(new EmailDomainValue("gmail.com")),
            new EmailDomain(new EmailDomainValue("outlook.com")),
            new EmailDomain(new EmailDomainValue("hotmail.com")),
            new EmailDomain(new EmailDomainValue("yahoo.com")),
            new EmailDomain(new EmailDomainValue("icloud.com"))
        };
        await _dbContext.EmailDomains.AddRangeAsync(domains);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedPhoneCodesAsync()
    {
        if (await _dbContext.PhoneCodes.AnyAsync()) return;
        var phoneCodes = new[]
        {
            new PhoneCode(new PhoneCodeValue("+57"), new PhoneCodeCountry("Colombia")),
            new PhoneCode(new PhoneCodeValue("+1"),  new PhoneCodeCountry("United States")),
            new PhoneCode(new PhoneCodeValue("+52"), new PhoneCodeCountry("Mexico")),
            new PhoneCode(new PhoneCodeValue("+34"), new PhoneCodeCountry("Spain")),
            new PhoneCode(new PhoneCodeValue("+55"), new PhoneCodeCountry("Brazil"))
        };
        await _dbContext.PhoneCodes.AddRangeAsync(phoneCodes);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedVehicleBrandsAsync()
    {
        if (await _dbContext.VehicleBrands.AnyAsync()) return;
        var brands = new[]
        {
            new VehicleBrand(new BrandName("Toyota")),
            new VehicleBrand(new BrandName("Chevrolet")),
            new VehicleBrand(new BrandName("Ford")),
            new VehicleBrand(new BrandName("Mazda")),
            new VehicleBrand(new BrandName("Renault")),
            new VehicleBrand(new BrandName("Kia")),
            new VehicleBrand(new BrandName("Hyundai")),
            new VehicleBrand(new BrandName("Volkswagen")),
            new VehicleBrand(new BrandName("Nissan")),
            new VehicleBrand(new BrandName("Honda"))
        };
        await _dbContext.VehicleBrands.AddRangeAsync(brands);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedVehicleModelsAsync()
    {
        if (await _dbContext.VehicleModels.AnyAsync()) return;

        var brands = await _dbContext.VehicleBrands.ToListAsync();
        var brandMap = brands.ToDictionary(b => b.BrandName.Value);
        var models = new List<VehicleModel>();

        if (brandMap.TryGetValue("Toyota", out var toyota))
            models.AddRange(new[]
            {
                new VehicleModel(toyota.Id, new ModelName("Corolla")),
                new VehicleModel(toyota.Id, new ModelName("Camry")),
                new VehicleModel(toyota.Id, new ModelName("Hilux")),
                new VehicleModel(toyota.Id, new ModelName("RAV4")),
                new VehicleModel(toyota.Id, new ModelName("Yaris"))
            });

        if (brandMap.TryGetValue("Chevrolet", out var chevrolet))
            models.AddRange(new[]
            {
                new VehicleModel(chevrolet.Id, new ModelName("Spark")),
                new VehicleModel(chevrolet.Id, new ModelName("Aveo")),
                new VehicleModel(chevrolet.Id, new ModelName("Cruze")),
                new VehicleModel(chevrolet.Id, new ModelName("Tracker")),
                new VehicleModel(chevrolet.Id, new ModelName("Captiva"))
            });

        if (brandMap.TryGetValue("Ford", out var ford))
            models.AddRange(new[]
            {
                new VehicleModel(ford.Id, new ModelName("Fiesta")),
                new VehicleModel(ford.Id, new ModelName("Focus")),
                new VehicleModel(ford.Id, new ModelName("Mustang")),
                new VehicleModel(ford.Id, new ModelName("Explorer")),
                new VehicleModel(ford.Id, new ModelName("Escape"))
            });

        if (brandMap.TryGetValue("Mazda", out var mazda))
            models.AddRange(new[]
            {
                new VehicleModel(mazda.Id, new ModelName("Mazda2")),
                new VehicleModel(mazda.Id, new ModelName("Mazda3")),
                new VehicleModel(mazda.Id, new ModelName("Mazda6")),
                new VehicleModel(mazda.Id, new ModelName("CX-5")),
                new VehicleModel(mazda.Id, new ModelName("CX-30"))
            });

        if (brandMap.TryGetValue("Renault", out var renault))
            models.AddRange(new[]
            {
                new VehicleModel(renault.Id, new ModelName("Sandero")),
                new VehicleModel(renault.Id, new ModelName("Duster")),
                new VehicleModel(renault.Id, new ModelName("Logan")),
                new VehicleModel(renault.Id, new ModelName("Kwid")),
                new VehicleModel(renault.Id, new ModelName("Captur"))
            });

        if (brandMap.TryGetValue("Kia", out var kia))
            models.AddRange(new[]
            {
                new VehicleModel(kia.Id, new ModelName("Picanto")),
                new VehicleModel(kia.Id, new ModelName("Rio")),
                new VehicleModel(kia.Id, new ModelName("Cerato")),
                new VehicleModel(kia.Id, new ModelName("Sportage")),
                new VehicleModel(kia.Id, new ModelName("Sorento"))
            });

        if (brandMap.TryGetValue("Hyundai", out var hyundai))
            models.AddRange(new[]
            {
                new VehicleModel(hyundai.Id, new ModelName("i10")),
                new VehicleModel(hyundai.Id, new ModelName("i20")),
                new VehicleModel(hyundai.Id, new ModelName("Elantra")),
                new VehicleModel(hyundai.Id, new ModelName("Tucson")),
                new VehicleModel(hyundai.Id, new ModelName("Santa Fe"))
            });

        if (brandMap.TryGetValue("Volkswagen", out var vw))
            models.AddRange(new[]
            {
                new VehicleModel(vw.Id, new ModelName("Polo")),
                new VehicleModel(vw.Id, new ModelName("Golf")),
                new VehicleModel(vw.Id, new ModelName("Jetta")),
                new VehicleModel(vw.Id, new ModelName("Tiguan")),
                new VehicleModel(vw.Id, new ModelName("Passat"))
            });

        if (brandMap.TryGetValue("Nissan", out var nissan))
            models.AddRange(new[]
            {
                new VehicleModel(nissan.Id, new ModelName("March")),
                new VehicleModel(nissan.Id, new ModelName("Sentra")),
                new VehicleModel(nissan.Id, new ModelName("Versa")),
                new VehicleModel(nissan.Id, new ModelName("X-Trail")),
                new VehicleModel(nissan.Id, new ModelName("Frontier"))
            });

        if (brandMap.TryGetValue("Honda", out var honda))
            models.AddRange(new[]
            {
                new VehicleModel(honda.Id, new ModelName("Fit")),
                new VehicleModel(honda.Id, new ModelName("Civic")),
                new VehicleModel(honda.Id, new ModelName("Accord")),
                new VehicleModel(honda.Id, new ModelName("CR-V")),
                new VehicleModel(honda.Id, new ModelName("HR-V"))
            });

        await _dbContext.VehicleModels.AddRangeAsync(models);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedVehicleColorsAsync()
    {
        if (await _dbContext.VehicleColors.AnyAsync()) return;
        var colors = new[]
        {
            new VehicleColor(new ColorName("White")),
            new VehicleColor(new ColorName("Black")),
            new VehicleColor(new ColorName("Silver")),
            new VehicleColor(new ColorName("Gray")),
            new VehicleColor(new ColorName("Red")),
            new VehicleColor(new ColorName("Blue")),
            new VehicleColor(new ColorName("Green")),
            new VehicleColor(new ColorName("Yellow"))
        };
        await _dbContext.VehicleColors.AddRangeAsync(colors);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedFuelTypesAsync()
    {
        if (await _dbContext.FuelTypes.AnyAsync()) return;
        var fuelTypes = new[]
        {
            new FuelType(new FuelTypeName("Gasoline")),
            new FuelType(new FuelTypeName("Diesel")),
            new FuelType(new FuelTypeName("Electric")),
            new FuelType(new FuelTypeName("Hybrid")),
            new FuelType(new FuelTypeName("Natural Gas"))
        };
        await _dbContext.FuelTypes.AddRangeAsync(fuelTypes);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedTransmissionTypesAsync()
    {
        if (await _dbContext.TransmissionTypes.AnyAsync()) return;
        var transmissionTypes = new[]
        {
            new TransmissionType(new TransmissionTypeName("Manual")),
            new TransmissionType(new TransmissionTypeName("Automatic")),
            new TransmissionType(new TransmissionTypeName("Semi-Automatic")),
            new TransmissionType(new TransmissionTypeName("CVT"))
        };
        await _dbContext.TransmissionTypes.AddRangeAsync(transmissionTypes);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedAuditActionTypesAsync()
    {
        if (await _dbContext.AuditActionTypes.AnyAsync()) return;
        var actionTypes = new[]
        {
            new AuditActionType(new AuditActionTypeName("Create")),
            new AuditActionType(new AuditActionTypeName("Update")),
            new AuditActionType(new AuditActionTypeName("Delete")),
            new AuditActionType(new AuditActionTypeName("Login")),
            new AuditActionType(new AuditActionTypeName("Logout"))
        };
        await _dbContext.AuditActionTypes.AddRangeAsync(actionTypes);
        await _dbContext.SaveChangesAsync();
    }
    private async Task SeedPartCategoriesAsync()
    {
        if (await _dbContext.PartCategories.AnyAsync()) return;

        var categories = new[]
        {
            new PartCategory(new PartCategoryName("Engine")),
            new PartCategory(new PartCategoryName("Brakes")),
            new PartCategory(new PartCategoryName("Suspension")),
            new PartCategory(new PartCategoryName("Electrical")),
            new PartCategory(new PartCategoryName("Transmission")),
            new PartCategory(new PartCategoryName("Cooling System")),
            new PartCategory(new PartCategoryName("Exhaust")),
            new PartCategory(new PartCategoryName("Filters")),
            new PartCategory(new PartCategoryName("Tires & Wheels")),
            new PartCategory(new PartCategoryName("Body & Exterior"))
        };

        await _dbContext.PartCategories.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedMeasurementUnitsAsync()
    {
        if (await _dbContext.MeasurementUnits.AnyAsync()) return;

        var units = new[]
        {
            new MeasurementUnit(new MeasurementUnitName("Unit"),       new MeasurementUnitAbbreviation("un")),
            new MeasurementUnit(new MeasurementUnitName("Liter"),      new MeasurementUnitAbbreviation("L")),
            new MeasurementUnit(new MeasurementUnitName("Milliliter"), new MeasurementUnitAbbreviation("mL")),
            new MeasurementUnit(new MeasurementUnitName("Kilogram"),   new MeasurementUnitAbbreviation("kg")),
            new MeasurementUnit(new MeasurementUnitName("Gram"),       new MeasurementUnitAbbreviation("g")),
            new MeasurementUnit(new MeasurementUnitName("Meter"),      new MeasurementUnitAbbreviation("m")),
            new MeasurementUnit(new MeasurementUnitName("Set"),        new MeasurementUnitAbbreviation("set")),
            new MeasurementUnit(new MeasurementUnitName("Pair"),       new MeasurementUnitAbbreviation("par"))
        };

        await _dbContext.MeasurementUnits.AddRangeAsync(units);
        await _dbContext.SaveChangesAsync();
    }
}
using Domain.Entities.Appointments;
using Domain.Entities.Parts;
using Domain.Entities.ServiceOrders;
using Domain.Entities.Quotations;
using Domain.Entities.Persons;
using Domain.Entities.Users;
using Domain.Entities.Vehicles;
using Domain.ValueObject.Appointments.Appointment;
using Domain.ValueObject.Appointments.AppointmentStatus;
using Domain.ValueObject.Parts.Part;
using Domain.ValueObject.Parts.PartCategory;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Quotations.Quotation;
using Domain.ValueObject.Quotations.QuotationStatus;
using Domain.ValueObject.ServiceOrders.OrderStatus;
using Domain.ValueObject.ServiceOrders.ServiceOrder;
using Domain.ValueObject.ServiceOrders.ServiceType;
using Domain.ValueObject.Users.Role;
using Domain.ValueObject.Users.User;
using Domain.ValueObject.Vehicles.Vehicle;
using Domain.ValueObject.Vehicles.VehicleBrand;
using Domain.ValueObject.Vehicles.VehicleModel;

namespace AutoTallerManager.Tests.Helpers;

internal static class SeedDataHelper
{
    // ── Order Statuses ─────────────────────────────────────────────
    internal static async Task<(int PendingId, int InProgressId, int CompletedId, int CancelledId)>
        SeedOrderStatusesAsync(AutoTallerDbContext db)
    {
        var pending    = new OrderStatus(new OrderStatusName("Pending"));
        var inProgress = new OrderStatus(new OrderStatusName("In Progress"));
        var completed  = new OrderStatus(new OrderStatusName("Completed"));
        var cancelled  = new OrderStatus(new OrderStatusName("Cancelled"));

        await db.OrderStatuses.AddRangeAsync(pending, inProgress, completed, cancelled);
        await db.SaveChangesAsync();

        return (pending.Id, inProgress.Id, completed.Id, cancelled.Id);
    }

    // ── Appointment Statuses ───────────────────────────────────────
    internal static async Task<int> SeedAppointmentStatusPendingAsync(AutoTallerDbContext db)
    {
        var status = new AppointmentStatus(new AppointmentStatusName("Pending"));
        await db.AppointmentStatuses.AddAsync(status);
        await db.SaveChangesAsync();
        return status.Id;
    }

    // ── Service Types ──────────────────────────────────────────────
    internal static async Task<(int DiagnosticsId, int RepairId)>
        SeedServiceTypesAsync(AutoTallerDbContext db)
    {
        var diagnostics = new ServiceType(new ServiceTypeName("Diagnostics"), new ServiceDuration(2));
        var repair      = new ServiceType(new ServiceTypeName("Repair"),      new ServiceDuration(8));

        await db.ServiceTypes.AddRangeAsync(diagnostics, repair);
        await db.SaveChangesAsync();

        return (diagnostics.Id, repair.Id);
    }

    // ── Quotation Statuses ─────────────────────────────────────────
    internal static async Task<(int PendingId, int AcceptedId, int RejectedId)>
        SeedQuotationStatusesAsync(AutoTallerDbContext db)
    {
        var pending  = new QuotationStatus(new QuotationStatusName("Pending"));
        var accepted = new QuotationStatus(new QuotationStatusName("Accepted"));
        var rejected = new QuotationStatus(new QuotationStatusName("Rejected"));

        await db.QuotationStatuses.AddRangeAsync(pending, accepted, rejected);
        await db.SaveChangesAsync();

        return (pending.Id, accepted.Id, rejected.Id);
    }

    // ── Vehicle Brand + Model ──────────────────────────────────────
    internal static async Task<(int BrandId, int ModelId)>
        SeedVehicleModelAsync(AutoTallerDbContext db, string brandName = "Toyota", string modelName = "Corolla")
    {
        var brand = new VehicleBrand(new BrandName(brandName));
        await db.VehicleBrands.AddAsync(brand);
        await db.SaveChangesAsync();

        var model = new VehicleModel(brand.Id, new ModelName(modelName));
        await db.VehicleModels.AddAsync(model);
        await db.SaveChangesAsync();

        return (brand.Id, model.Id);
    }

    // ── Vehicle ───────────────────────────────────────────────────
    internal static async Task<int> SeedVehicleAsync(
        AutoTallerDbContext db, int modelId, string vin = "1HGCM82633A000001")
    {
        var vehicle = new Vehicle(
            modelId,
            new VinNumber(vin),
            new VehicleYear(2020),
            new VehicleMileage(15_000));

        await db.Vehicles.AddAsync(vehicle);
        await db.SaveChangesAsync();
        return vehicle.Id;
    }

    // ── Mechanic User ──────────────────────────────────────────────
    internal static async Task<(int PersonId, int UserId)>
        SeedMechanicAsync(AutoTallerDbContext db)
    {
        var person = new Person(new PersonFirstName("Carlos"), new PersonLastName("Mechanic"));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var user = new User(person.Id, new PasswordHash(BCrypt.Net.BCrypt.HashPassword("Pass123!")));
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return (person.Id, user.Id);
    }

    // ── Part Category ──────────────────────────────────────────────
    internal static async Task<int> SeedPartCategoryAsync(
        AutoTallerDbContext db, string name = "Engine Parts")
    {
        var category = new PartCategory(new PartCategoryName(name));
        await db.PartCategories.AddAsync(category);
        await db.SaveChangesAsync();
        return category.Id;
    }

    // ── Part ───────────────────────────────────────────────────────
    internal static async Task<int> SeedPartAsync(
        AutoTallerDbContext db, int categoryId,
        string code = "ENG-001", int stock = 10, int minStock = 2, decimal unitPrice = 150m)
    {
        var part = new Part(
            categoryId,
            new PartCode(code),
            new PartDescription("Test part"),
            new PartStock(stock),
            new PartMinStock(minStock),
            new PartUnitPrice(unitPrice));

        await db.Parts.AddAsync(part);
        await db.SaveChangesAsync();
        return part.Id;
    }

    // ── Service Order ──────────────────────────────────────────────
    internal static async Task<int> SeedServiceOrderAsync(
        AutoTallerDbContext db,
        int vehicleId, int serviceTypeId, int mechanicId, int orderStatusId)
    {
        var order = new ServiceOrder(
            vehicleId, serviceTypeId, mechanicId, orderStatusId,
            new WorkDescription("Test work"),
            new ServiceOrderNotes("Test notes"));

        await db.ServiceOrders.AddAsync(order);
        await db.SaveChangesAsync();
        return order.Id;
    }

    // ── Quotation ─────────────────────────────────────────────────
    internal static async Task<int> SeedQuotationAsync(
        AutoTallerDbContext db,
        int serviceOrderId, int createdByUserId, int quotationStatusId,
        decimal laborCost = 100m)
    {
        var quotation = new Quotation(
            serviceOrderId, createdByUserId, quotationStatusId,
            new LaborCost(laborCost),
            new QuotationSubtotal(laborCost),
            new QuotationTotal(laborCost),
            new QuotationNotes("Test quotation"));

        await db.Quotations.AddAsync(quotation);
        await db.SaveChangesAsync();
        return quotation.Id;
    }

    // ── Appointment ───────────────────────────────────────────────
    internal static async Task<int> SeedAppointmentAsync(
        AutoTallerDbContext db,
        int customerId, int vehicleId, int serviceTypeId, int appointmentStatusId)
    {
        var appointment = new Appointment(
            customerId, vehicleId, serviceTypeId, appointmentStatusId,
            new AppointmentDate(DateTime.UtcNow.AddDays(1)),
            new AppointmentNotes("Test appointment"));

        await db.Appointments.AddAsync(appointment);
        await db.SaveChangesAsync();
        return appointment.Id;
    }
}

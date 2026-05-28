using Domain.Entities.ServiceOrders;
using Domain.ValueObject.ServiceOrders.OrderStatus;
using Domain.ValueObject.ServiceOrders.ServiceType;
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

        await SeedOrderStatusesAsync();
        await SeedServiceTypesAsync();
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
}

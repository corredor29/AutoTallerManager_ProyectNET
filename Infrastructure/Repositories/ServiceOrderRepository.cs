using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.ServiceOrders;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ServiceOrderRepository : IServiceOrderRepository
{
    private readonly AutoTallerDbContext _dbContext;

    public ServiceOrderRepository(AutoTallerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ServiceOrder>> GetAllPagedAsync(
        PaginationParams pagination, ServiceOrderFilter filter)
    {
        var query = _dbContext.ServiceOrders
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.Appointment)
                .ThenInclude(x => x.Customer)
                    .ThenInclude(x => x.Person)
            .Include(x => x.ServiceType)
            .Include(x => x.OrderStatus)
            .Include(x => x.Mechanic)
                .ThenInclude(x => x.Person)
            .AsQueryable();

        query = query
            .WhereIf(filter.OrderStatusId.HasValue, x => x.OrderStatusId == filter.OrderStatusId!.Value)
            .WhereIf(filter.MechanicId.HasValue,    x => x.MechanicId    == filter.MechanicId!.Value)
            .WhereIf(filter.ServiceTypeId.HasValue, x => x.ServiceTypeId == filter.ServiceTypeId!.Value)
            .WhereIf(filter.CustomerId.HasValue,
                x => x.Appointment != null && x.Appointment.CustomerId == filter.CustomerId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CustomerName),
                x => x.Appointment != null &&
                    EF.Functions.ILike(
                        (x.Appointment.Customer.Person.FirstName.Value + " " + x.Appointment.Customer.Person.LastName.Value).Trim(),
                        "%" + filter.CustomerName!.Trim() + "%"))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.VehicleVin),
                x => EF.Functions.ILike(x.Vehicle.VIN.Value, "%" + filter.VehicleVin!.Trim() + "%"))
            .WhereIf(filter.DateFrom.HasValue,      x => x.CreatedAt >= filter.DateFrom!.Value)
            .WhereIf(filter.DateTo.HasValue,
                x => x.CreatedAt <= filter.DateTo!.Value.Date.AddDays(1).AddTicks(-1));

        var totalCount = await query.CountAsync();
        var page = pagination.NormalizedPageNumber;
        var size = pagination.NormalizedPageSize;

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResult<ServiceOrder>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = size,
            TotalCount = totalCount
        };
    }

    public async Task<ServiceOrder?> GetByIdAsync(int id)
    {
        return await _dbContext.ServiceOrders
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.Appointment)
                .ThenInclude(x => x.Customer)
                    .ThenInclude(x => x.Person)
            .Include(x => x.ServiceType)
            .Include(x => x.OrderStatus)
            .Include(x => x.Mechanic)
                .ThenInclude(x => x.Person)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ServiceOrder>> GetAllAsync()
    {
        return await _dbContext.ServiceOrders
            .Include(x => x.Vehicle)
                .ThenInclude(x => x.Model)
                    .ThenInclude(x => x.Brand)
            .Include(x => x.Appointment)
                .ThenInclude(x => x.Customer)
                    .ThenInclude(x => x.Person)
            .Include(x => x.ServiceType)
            .Include(x => x.OrderStatus)
            .Include(x => x.Mechanic)
                .ThenInclude(x => x.Person)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasOpenOrderForVehicleAsync(int vehicleId, int? excludeId = null)
    {
        return await _dbContext.ServiceOrders.AnyAsync(x =>
            x.VehicleId == vehicleId &&
            x.ClosedAt == null &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<bool> HasActiveOrderForVehicleAsync(int vehicleId)
    {
        var allStatuses = await _dbContext.OrderStatuses.ToListAsync();
        var activeStatusIds = allStatuses
            .Where(s => s.Name.Value.ToLower() == "pending" ||
                        s.Name.Value.ToLower() == "in progress")
            .Select(s => s.Id)
            .ToList();

        return await _dbContext.ServiceOrders.AnyAsync(x =>
            x.VehicleId == vehicleId &&
            activeStatusIds.Contains(x.OrderStatusId));
    }

    public async Task AddAsync(ServiceOrder serviceOrder)
    {
        await _dbContext.ServiceOrders.AddAsync(serviceOrder);
    }

    public void Update(ServiceOrder serviceOrder)
    {
        _dbContext.ServiceOrders.Update(serviceOrder);
    }

    public void Remove(ServiceOrder serviceOrder)
    {
        _dbContext.ServiceOrders.Remove(serviceOrder);
    }
}

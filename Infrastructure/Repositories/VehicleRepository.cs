using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Vehicles;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class VehicleRepository : IVehicleRepository
    {
        private readonly AutoTallerDbContext _dbContext;

        public VehicleRepository(AutoTallerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Vehicle>> GetAllPagedAsync(
            PaginationParams pagination, VehicleFilter filter)
        {
            var allVehicles = await _dbContext.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .Include(v => v.Ownerships)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.Person)
                .ToListAsync();

            var query = allVehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.VIN))
                query = query.Where(v => v.VIN.Value
                    .Contains(filter.VIN.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.LicensePlate))
                query = query.Where(v => v.LicensePlate != null &&
                    v.LicensePlate.Value.Contains(filter.LicensePlate.Trim(), StringComparison.OrdinalIgnoreCase));

            if (filter.BrandId.HasValue)
                query = query.Where(v => v.Model.BrandId == filter.BrandId.Value);

            if (filter.ModelId.HasValue)
                query = query.Where(v => v.ModelId == filter.ModelId.Value);

            if (filter.Year.HasValue)
                query = query.Where(v => v.Year.Value == filter.Year.Value);

            var totalCount = query.Count();
            var page       = pagination.NormalizedPageNumber;
            var size       = pagination.NormalizedPageSize;

            var items = query
                .OrderBy(v => v.Model.Brand.BrandName.Value)
                .ThenBy(v => v.Model.ModelName.Value)
                .ThenBy(v => v.VIN.Value)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new PagedResult<Vehicle>
            {
                Items      = items,
                PageNumber = page,
                PageSize   = size,
                TotalCount = totalCount
            };
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _dbContext.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .Include(v => v.Ownerships)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.Person)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _dbContext.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .Include(v => v.Ownerships)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.Person)
                .ToListAsync();
        }

        public async Task<bool> ExistsByVinAsync(string vin, int? excludeId = null)
        {
            var normalizedVin = vin.Trim().ToUpperInvariant();
            var all = await _dbContext.Vehicles.ToListAsync();
            return all.Any(v =>
                v.VIN.Value.ToUpperInvariant() == normalizedVin &&
                (!excludeId.HasValue || v.Id != excludeId.Value));
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _dbContext.Vehicles.AddAsync(vehicle);
        }

        public void Update(Vehicle vehicle)
        {
            _dbContext.Vehicles.Update(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            _dbContext.Vehicles.Remove(vehicle);
        }
    }
}
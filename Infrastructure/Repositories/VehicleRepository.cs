using Application.Common.Pagination;
using Application.Contracts.Repositories;
using Application.Filters;
using Domain.Entities.Vehicles;
using Infrastructure.Context;
using Infrastructure.Extensions;
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
            var query = _dbContext.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Color)
                .Include(v => v.FuelType)
                .Include(v => v.TransmissionType)
                .AsQueryable();

            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(filter.VIN),
                    v => EF.Functions.ILike(v.VIN.Value, "%" + filter.VIN!.Trim() + "%"))
                .WhereIf(!string.IsNullOrWhiteSpace(filter.LicensePlate),
                    v => v.LicensePlate != null &&
                         EF.Functions.ILike(v.LicensePlate.Value, "%" + filter.LicensePlate!.Trim() + "%"))
                .WhereIf(filter.BrandId.HasValue, v => v.Model.BrandId == filter.BrandId!.Value)
                .WhereIf(filter.ModelId.HasValue, v => v.ModelId == filter.ModelId!.Value)
                .WhereIf(filter.Year.HasValue,    v => v.Year.Value == filter.Year!.Value);

            var totalCount = await query.CountAsync();
            var page = pagination.NormalizedPageNumber;
            var size = pagination.NormalizedPageSize;

            var items = await query
                .OrderBy(v => v.Model.Brand.BrandName.Value)
                .ThenBy(v => v.Model.ModelName.Value)
                .ThenBy(v => v.VIN.Value)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

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
                .ToListAsync();
        }

        public async Task<bool> ExistsByVinAsync(string vin, int? excludeId = null)
        {
            var normalizedVin = vin.Trim().ToUpperInvariant();
            return await _dbContext.Vehicles.AnyAsync(v =>
                v.VIN.Value == normalizedVin &&
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

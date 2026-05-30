using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Vehicles;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class VehicleOwnershipHistoryRepository : IVehicleOwnershipHistoryRepository
    {
        private readonly AutoTallerDbContext _context;

        public VehicleOwnershipHistoryRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleOwnershipHistory?> GetByIdAsync(int id)
            => await _context.VehicleOwnershipHistories
                            .Include(x => x.Vehicle)
                            .Include(x => x.Customer)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<VehicleOwnershipHistory>> GetByVehicleIdAsync(int vehicleId)
            => await _context.VehicleOwnershipHistories
                            .Include(x => x.Customer)
                            .Where(x => x.VehicleId == vehicleId)
                            .ToListAsync();

        public async Task<IEnumerable<VehicleOwnershipHistory>> GetByCustomerIdAsync(int customerId)
            => await _context.VehicleOwnershipHistories
                            .Include(x => x.Vehicle)
                            .Where(x => x.CustomerId == customerId)
                            .ToListAsync();

        public async Task<VehicleOwnershipHistory?> GetCurrentOwnerAsync(int vehicleId)
            => await _context.VehicleOwnershipHistories
                            .Include(x => x.Customer)
                            .FirstOrDefaultAsync(x => x.VehicleId == vehicleId
                                                    && x.DateRange.EndDate == null);

        public async Task<bool> HasActiveOwnershipAsync(int vehicleId)
            => await _context.VehicleOwnershipHistories
                            .AnyAsync(x => x.VehicleId == vehicleId
                                        && x.DateRange.EndDate == null);

        public async Task AddAsync(VehicleOwnershipHistory ownership)
            => await _context.VehicleOwnershipHistories.AddAsync(ownership);

        public void Update(VehicleOwnershipHistory ownership)
            => _context.VehicleOwnershipHistories.Update(ownership);
    }
}
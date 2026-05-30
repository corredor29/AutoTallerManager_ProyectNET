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
    public sealed class MileageHistoryRepository : IMileageHistoryRepository
    {
        private readonly AutoTallerDbContext _context;

        public MileageHistoryRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<MileageHistory?> GetByIdAsync(int id)
            => await _context.MileageHistories
                            .Include(x => x.Vehicle)
                            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<MileageHistory>> GetByVehicleIdAsync(int vehicleId)
            => await _context.MileageHistories
                            .Where(x => x.VehicleId == vehicleId)
                            .OrderByDescending(x => x.RecordedAt)
                            .ToListAsync();

        public async Task<MileageHistory?> GetLatestByVehicleIdAsync(int vehicleId)
            => await _context.MileageHistories
                            .Where(x => x.VehicleId == vehicleId)
                            .OrderByDescending(x => x.RecordedAt)
                            .FirstOrDefaultAsync();

        public async Task AddAsync(MileageHistory mileageHistory)
            => await _context.MileageHistories.AddAsync(mileageHistory);

        public void Update(MileageHistory mileageHistory)
            => _context.MileageHistories.Update(mileageHistory);

        public void Remove(MileageHistory mileageHistory)
            => _context.MileageHistories.Remove(mileageHistory);
    }
}
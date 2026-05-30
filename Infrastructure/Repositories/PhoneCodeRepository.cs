using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities.Persons;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class PhoneCodeRepository : IPhoneCodeRepository
    {
        private readonly AutoTallerDbContext _context;

        public PhoneCodeRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<PhoneCode?> GetByIdAsync(int id)
            => await _context.PhoneCodes.FindAsync(id);

        public async Task<IEnumerable<PhoneCode>> GetAllAsync()
            => await _context.PhoneCodes.ToListAsync();

        public async Task<bool> ExistsByCodeAsync(string code)
            => await _context.PhoneCodes.AnyAsync(x => x.Code.Value == code);

        public async Task AddAsync(PhoneCode phoneCode)
            => await _context.PhoneCodes.AddAsync(phoneCode);

        public void Update(PhoneCode phoneCode)
            => _context.PhoneCodes.Update(phoneCode);

        public void Remove(PhoneCode phoneCode)
            => _context.PhoneCodes.Remove(phoneCode);
    }
}
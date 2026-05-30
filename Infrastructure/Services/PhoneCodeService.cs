using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PhoneCodes;
using Application.Requests.PhoneCodes;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.PhoneCode;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{

    public sealed class PhoneCodeService : IPhoneCodeService
    {
        private readonly IPhoneCodeRepository _phoneCodeRepository;
        private readonly AutoTallerDbContext  _dbContext;

        public PhoneCodeService(IPhoneCodeRepository phoneCodeRepository,
                                AutoTallerDbContext dbContext)
        {
            _phoneCodeRepository = phoneCodeRepository;
            _dbContext           = dbContext;
        }

        public async Task<IEnumerable<PhoneCodeDto>> GetAllAsync()
        {
            var phoneCodes = await _phoneCodeRepository.GetAllAsync();
            return phoneCodes.Select(MapToDto);
        }

        public async Task<PhoneCodeDto?> GetByIdAsync(int id)
        {
            var phoneCode = await _phoneCodeRepository.GetByIdAsync(id);
            return phoneCode is null ? null : MapToDto(phoneCode);
        }

        public async Task<PhoneCodeDto> CreateAsync(CreatePhoneCodeRequest request)
        {
            await EnsureCodeIsUniqueAsync(request.Code);

            var phoneCode = new PhoneCode(
                new PhoneCodeValue(request.Code),
                new PhoneCodeCountry(request.Country)
            );

            await _phoneCodeRepository.AddAsync(phoneCode);
            await _dbContext.SaveChangesAsync();

            return MapToDto(phoneCode);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePhoneCodeRequest request)
        {
            var phoneCode = await _phoneCodeRepository.GetByIdAsync(id);
            if (phoneCode is null) return false;

            await EnsureCodeIsUniqueAsync(request.Code, id);

            phoneCode.Update(
                new PhoneCodeValue(request.Code),
                new PhoneCodeCountry(request.Country)
            );

            _phoneCodeRepository.Update(phoneCode);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var phoneCode = await _phoneCodeRepository.GetByIdAsync(id);
            if (phoneCode is null) return false;

            if (await _dbContext.PersonPhones.AnyAsync(x => x.PhoneCodeId == id))
                throw new InvalidOperationException($"Phone code {id} is being used and cannot be deleted.");

            _phoneCodeRepository.Remove(phoneCode);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureCodeIsUniqueAsync(string code, int? excludeId = null)
        {
            var normalizedCode = code.Trim().ToUpper();
            var exists = await _dbContext.PhoneCodes.AnyAsync(x =>
                x.Code.Value.ToUpper() == normalizedCode &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Phone code '{code}' already exists.");
        }

        private static PhoneCodeDto MapToDto(PhoneCode phoneCode) => new()
        {
            Id      = phoneCode.Id,
            Code    = phoneCode.Code.Value,
            Country = phoneCode.Country.Value
        };
    }
}
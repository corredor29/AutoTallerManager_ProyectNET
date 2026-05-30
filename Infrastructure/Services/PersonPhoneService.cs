using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PersonPhones;
using Application.Requests.PersonPhones;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.PersonPhone;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Services
{

    public sealed class PersonPhoneService : IPersonPhoneService
    {
        private readonly IPersonPhoneRepository _personPhoneRepository;
        private readonly AutoTallerDbContext     _dbContext;

        public PersonPhoneService(IPersonPhoneRepository personPhoneRepository,
                                AutoTallerDbContext dbContext)
        {
            _personPhoneRepository = personPhoneRepository;
            _dbContext             = dbContext;
        }

        public async Task<IEnumerable<PersonPhoneDto>> GetByPersonIdAsync(int personId)
        {
            var phones = await _personPhoneRepository.GetByPersonIdAsync(personId);
            return phones.Select(MapToDto);
        }

        public async Task<PersonPhoneDto?> GetByIdAsync(int id)
        {
            var phone = await _personPhoneRepository.GetByIdAsync(id);
            return phone is null ? null : MapToDto(phone);
        }

        public async Task<PersonPhoneDto> CreateAsync(CreatePersonPhoneRequest request)
        {
            await EnsurePhoneIsUniqueAsync(request.PhoneCodeId, request.PhoneNumber);

            if (request.IsPrimary)
                await ResetPrimaryAsync(request.PersonId);

            var phone = new PersonPhone(
                request.PersonId,
                request.PhoneCodeId,
                new PhoneNumber(request.PhoneNumber),
                request.IsPrimary
            );

            await _personPhoneRepository.AddAsync(phone);
            await _dbContext.SaveChangesAsync();

            return MapToDto(phone);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePersonPhoneRequest request)
        {
            var phone = await _personPhoneRepository.GetByIdAsync(id);
            if (phone is null) return false;

            await EnsurePhoneIsUniqueAsync(request.PhoneCodeId, request.PhoneNumber, id);

            if (request.IsPrimary)
                await ResetPrimaryAsync(phone.PersonId, id);

            phone.Update(
                new PhoneNumber(request.PhoneNumber),
                request.IsPrimary
            );

            _personPhoneRepository.Update(phone);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var phone = await _personPhoneRepository.GetByIdAsync(id);
            if (phone is null) return false;

            _personPhoneRepository.Remove(phone);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetAsPrimaryAsync(int id)
        {
            var phone = await _personPhoneRepository.GetByIdAsync(id);
            if (phone is null) return false;

            await ResetPrimaryAsync(phone.PersonId, id);

            phone.SetAsPrimary();
            _personPhoneRepository.Update(phone);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsurePhoneIsUniqueAsync(int phoneCodeId, string phoneNumber, int? excludeId = null)
        {
            var exists = await _dbContext.PersonPhones.AnyAsync(x =>
                x.PhoneCodeId == phoneCodeId &&
                x.PhoneNumber.Value == phoneNumber &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Phone number '{phoneNumber}' already exists for this phone code.");
        }

        private async Task ResetPrimaryAsync(int personId, int? excludeId = null)
        {
            var primaryPhones = await _dbContext.PersonPhones
                .Where(x => x.PersonId == personId
                        && x.IsPrimary == true
                        && (!excludeId.HasValue || x.Id != excludeId.Value))
                .ToListAsync();

            foreach (var p in primaryPhones)
            {
                p.SetAsSecondary();
                _dbContext.PersonPhones.Update(p);
            }
        }

        private static PersonPhoneDto MapToDto(PersonPhone phone) => new()
        {
            Id          = phone.Id,
            PersonId    = phone.PersonId,
            PhoneCodeId = phone.PhoneCodeId,
            PhoneNumber = phone.PhoneNumber.Value,
            PhoneCode   = phone.PhoneCode?.Code.Value    ?? string.Empty,
            Country     = phone.PhoneCode?.Country.Value ?? string.Empty,
            FullPhone   = $"{phone.PhoneCode?.Code.Value ?? string.Empty}{phone.PhoneNumber.Value}",
            IsPrimary   = phone.IsPrimary
        };
    }
}
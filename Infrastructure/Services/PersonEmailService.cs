using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PersonEmails;
using Application.Requests.PersonEmails;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.PersonEmail;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class PersonEmailService : IPersonEmailService
    {
        private readonly IPersonEmailRepository _personEmailRepository;
        private readonly AutoTallerDbContext     _dbContext;

        public PersonEmailService(IPersonEmailRepository personEmailRepository,
                                AutoTallerDbContext dbContext)
        {
            _personEmailRepository = personEmailRepository;
            _dbContext             = dbContext;
        }

        public async Task<IEnumerable<PersonEmailDto>> GetByPersonIdAsync(int personId)
        {
            var emails = await _personEmailRepository.GetByPersonIdAsync(personId);
            return emails.Select(MapToDto);
        }

        public async Task<PersonEmailDto?> GetByIdAsync(int id)
        {
            var email = await _personEmailRepository.GetByIdAsync(id);
            return email is null ? null : MapToDto(email);
        }

        public async Task<PersonEmailDto> CreateAsync(CreatePersonEmailRequest request)
        {
            await EnsureEmailIsUniqueAsync(request.EmailUser, request.EmailDomainId);

            if (request.IsPrimary)
                await ResetPrimaryAsync(request.PersonId);

            var email = new PersonEmail(
                request.PersonId,
                request.EmailDomainId,
                new EmailUser(request.EmailUser),
                request.IsPrimary
            );

            await _personEmailRepository.AddAsync(email);
            await _dbContext.SaveChangesAsync();

            return MapToDto(email);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePersonEmailRequest request)
        {
            var email = await _personEmailRepository.GetByIdAsync(id);
            if (email is null) return false;

            await EnsureEmailIsUniqueAsync(request.EmailUser, request.EmailDomainId, id);

            if (request.IsPrimary)
                await ResetPrimaryAsync(email.PersonId, id);

            email.Update(
                new EmailUser(request.EmailUser),
                request.IsPrimary
            );

            _personEmailRepository.Update(email);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var email = await _personEmailRepository.GetByIdAsync(id);
            if (email is null) return false;

            _personEmailRepository.Remove(email);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetAsPrimaryAsync(int id)
        {
            var email = await _personEmailRepository.GetByIdAsync(id);
            if (email is null) return false;

            await ResetPrimaryAsync(email.PersonId, id);

            email.SetAsPrimary();
            _personEmailRepository.Update(email);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureEmailIsUniqueAsync(string emailUser, int emailDomainId, int? excludeId = null)
        {
            var exists = await _dbContext.PersonEmails.AnyAsync(x =>
                x.EmailUser.Value == emailUser &&
                x.EmailDomainId == emailDomainId &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Email '{emailUser}' already exists for this domain.");
        }

        private async Task ResetPrimaryAsync(int personId, int? excludeId = null)
        {
            var primaryEmails = await _dbContext.PersonEmails
                .Where(x => x.PersonId == personId
                        && x.IsPrimary == true
                        && (!excludeId.HasValue || x.Id != excludeId.Value))
                .ToListAsync();

            foreach (var e in primaryEmails)
            {
                e.SetAsSecondary();
                _dbContext.PersonEmails.Update(e);
            }
        }

        private static PersonEmailDto MapToDto(PersonEmail email) => new()
        {
            Id            = email.Id,
            PersonId      = email.PersonId,
            EmailDomainId = email.EmailDomainId,
            EmailUser     = email.EmailUser.Value,
            Domain        = email.EmailDomain?.Domain.Value ?? string.Empty,
            FullEmail     = $"{email.EmailUser.Value}@{email.EmailDomain?.Domain.Value ?? string.Empty}",
            IsPrimary     = email.IsPrimary
        };
    }
}
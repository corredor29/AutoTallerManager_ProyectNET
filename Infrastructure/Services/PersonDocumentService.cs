using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.PersonDocuments;
using Application.Requests.PersonDocuments;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.PersonDocument;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class PersonDocumentService : IPersonDocumentService
    {
        private readonly IPersonDocumentRepository _personDocumentRepository;
        private readonly AutoTallerDbContext        _dbContext;

        public PersonDocumentService(IPersonDocumentRepository personDocumentRepository,
                                    AutoTallerDbContext dbContext)
        {
            _personDocumentRepository = personDocumentRepository;
            _dbContext                = dbContext;
        }

        public async Task<IEnumerable<PersonDocumentDto>> GetByPersonIdAsync(int personId)
        {
            var documents = await _personDocumentRepository.GetByPersonIdAsync(personId);
            return documents.Select(MapToDto);
        }

        public async Task<PersonDocumentDto?> GetByIdAsync(int id)
        {
            var document = await _personDocumentRepository.GetByIdAsync(id);
            return document is null ? null : MapToDto(document);
        }

        public async Task<PersonDocumentDto> CreateAsync(CreatePersonDocumentRequest request)
        {
            await EnsureDocumentIsUniqueAsync(request.DocumentTypeId, request.DocumentNumber);

            var document = new PersonDocument(
                request.PersonId,
                request.DocumentTypeId,
                new DocumentNumber(request.DocumentNumber),
                request.IsPrimary
            );

            if (request.IsPrimary)
                await ResetPrimaryAsync(request.PersonId);

            await _personDocumentRepository.AddAsync(document);
            await _dbContext.SaveChangesAsync();

            return MapToDto(document);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePersonDocumentRequest request)
        {
            var document = await _personDocumentRepository.GetByIdAsync(id);
            if (document is null) return false;

            await EnsureDocumentIsUniqueAsync(request.DocumentTypeId, request.DocumentNumber, id);

            if (request.IsPrimary)
                await ResetPrimaryAsync(document.PersonId, id);

            document.Update(
                new DocumentNumber(request.DocumentNumber),
                request.IsPrimary
            );

            _personDocumentRepository.Update(document);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _personDocumentRepository.GetByIdAsync(id);
            if (document is null) return false;

            _personDocumentRepository.Remove(document);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetAsPrimaryAsync(int id)
        {
            var document = await _personDocumentRepository.GetByIdAsync(id);
            if (document is null) return false;

            await ResetPrimaryAsync(document.PersonId, id);

            document.SetAsPrimary();
            _personDocumentRepository.Update(document);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureDocumentIsUniqueAsync(int documentTypeId, string documentNumber, int? excludeId = null)
        {
            var exists = await _dbContext.PersonDocuments.AnyAsync(x =>
                x.DocumentTypeId == documentTypeId &&
                x.DocumentNumber.Value == documentNumber &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Document number '{documentNumber}' already exists for this document type.");
        }

        private async Task ResetPrimaryAsync(int personId, int? excludeId = null)
        {
            var primaryDocuments = await _dbContext.PersonDocuments
                .Where(x => x.PersonId == personId
                        && x.IsPrimary == true
                        && (!excludeId.HasValue || x.Id != excludeId.Value))
                .ToListAsync();

            foreach (var doc in primaryDocuments)
            {
                doc.SetAsSecondary();
                _dbContext.PersonDocuments.Update(doc);
            }
        }

        private static PersonDocumentDto MapToDto(PersonDocument document) => new()
        {
            Id             = document.Id,
            PersonId       = document.PersonId,
            DocumentTypeId = document.DocumentTypeId,
            DocumentNumber = document.DocumentNumber.Value,
            IsPrimary      = document.IsPrimary,
            DocumentType   = document.DocumentType?.Name.Value ?? string.Empty,
            DocumentCode   = document.DocumentType?.Code.Value ?? string.Empty
        };
    }
}
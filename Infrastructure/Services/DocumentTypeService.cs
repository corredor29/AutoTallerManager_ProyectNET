using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.DocumentTypes;
using Application.Requests.DocumentTypes;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.DocumentType;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Services
{
    public sealed class DocumentTypeService : IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly AutoTallerDbContext     _dbContext;

        public DocumentTypeService(IDocumentTypeRepository documentTypeRepository,
                                    AutoTallerDbContext dbContext)
        {
            _documentTypeRepository = documentTypeRepository;
            _dbContext              = dbContext;
        }

        public async Task<IEnumerable<DocumentTypeDto>> GetAllAsync()
        {
            var documentTypes = await _documentTypeRepository.GetAllAsync();
            return documentTypes.Select(MapToDto);
        }

        public async Task<DocumentTypeDto?> GetByIdAsync(int id)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            return documentType is null ? null : MapToDto(documentType);
        }

        public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeRequest request)
        {
            await EnsureCodeIsUniqueAsync(request.Code);

            var documentType = new DocumentType(
                new DocumentCode(request.Code),
                new DocumentTypeName(request.Name)
            );

            await _documentTypeRepository.AddAsync(documentType);
            await _dbContext.SaveChangesAsync();

            return MapToDto(documentType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateDocumentTypeRequest request)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType is null) return false;

            await EnsureCodeIsUniqueAsync(request.Code, id);

            documentType.Update(
                new DocumentCode(request.Code),
                new DocumentTypeName(request.Name)
            );

            _documentTypeRepository.Update(documentType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType is null) return false;

            if (await _dbContext.PersonDocuments.AnyAsync(x => x.DocumentTypeId == id))
                throw new InvalidOperationException($"Document type {id} is being used and cannot be deleted.");

            _documentTypeRepository.Remove(documentType);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureCodeIsUniqueAsync(string code, int? excludeId = null)
        {
            var normalizedCode = code.Trim().ToUpper();
            var exists = await _dbContext.DocumentTypes.AnyAsync(x =>
                x.Code.Value.ToUpper() == normalizedCode &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Document type with code '{code}' already exists.");
        }

        private static DocumentTypeDto MapToDto(DocumentType documentType) => new()
        {
            Id   = documentType.Id,
            Code = documentType.Code.Value,
            Name = documentType.Name.Value
        };
    }
}
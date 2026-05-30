using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.DocumentTypes;
using Application.Requests.DocumentTypes;

namespace Application.Contracts.Services
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentTypeDto>> GetAllAsync();
        Task<DocumentTypeDto?> GetByIdAsync(int id);
        Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeRequest request);
        Task<bool> UpdateAsync(int id, UpdateDocumentTypeRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
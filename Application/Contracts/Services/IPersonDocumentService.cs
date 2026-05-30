using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PersonDocuments;
using Application.Requests.PersonDocuments;
namespace Application.Contracts.Services
{
    public interface IPersonDocumentService
    {
        Task<IEnumerable<PersonDocumentDto>> GetByPersonIdAsync(int personId);
        Task<PersonDocumentDto?> GetByIdAsync(int id);
        Task<PersonDocumentDto> CreateAsync(CreatePersonDocumentRequest request);
        Task<bool> UpdateAsync(int id, UpdatePersonDocumentRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetAsPrimaryAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PersonEmails;
using Application.Requests.PersonEmails;


namespace Application.Contracts.Services
{
    public interface IPersonEmailService
    {
        Task<IEnumerable<PersonEmailDto>> GetByPersonIdAsync(int personId);
        Task<PersonEmailDto?> GetByIdAsync(int id);
        Task<PersonEmailDto>  CreateAsync(CreatePersonEmailRequest request);
        Task<bool> UpdateAsync(int id, UpdatePersonEmailRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetAsPrimaryAsync(int id);
    }
}
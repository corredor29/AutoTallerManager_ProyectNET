using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PersonPhones;
using Application.Requests.PersonPhones;

namespace Application.Contracts.Services
{
    public interface IPersonPhoneService
    {
        Task<IEnumerable<PersonPhoneDto>> GetByPersonIdAsync(int personId);
        Task<PersonPhoneDto?>             GetByIdAsync(int id);
        Task<PersonPhoneDto>              CreateAsync(CreatePersonPhoneRequest request);
        Task<bool>                        UpdateAsync(int id, UpdatePersonPhoneRequest request);
        Task<bool>                        DeleteAsync(int id);
        Task<bool>                        SetAsPrimaryAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Persons;
using Application.Requests.Persons;

namespace Application.Contracts.Services
{
public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetAllAsync();
    Task<PersonDto?> GetByIdAsync(int id);
    Task<PersonDto> CreateAsync(CreatePersonRequest request);
    Task<bool> UpdateAsync(int id, UpdatePersonRequest request);
    Task<bool> DeleteAsync(int id);
}
}
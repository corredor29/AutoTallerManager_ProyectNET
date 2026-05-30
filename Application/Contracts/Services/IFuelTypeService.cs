using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.FuelTypes;
using Application.Requests.FuelTypes;

namespace Application.Contracts.Services
{
    public interface IFuelTypeService
    {
        Task<IEnumerable<FuelTypeDto>> GetAllAsync();
        Task<FuelTypeDto?>             GetByIdAsync(int id);
        Task<FuelTypeDto>              CreateAsync(CreateFuelTypeRequest request);
        Task<bool>                     UpdateAsync(int id, UpdateFuelTypeRequest request);
        Task<bool>                     DeleteAsync(int id);
    }
}
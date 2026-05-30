using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.TransmissionTypes;
using Application.Requests.TransmissionTypes;


namespace Application.Contracts.Services
{
    public interface ITransmissionTypeService
    {
        Task<IEnumerable<TransmissionTypeDto>> GetAllAsync();
        Task<TransmissionTypeDto?>             GetByIdAsync(int id);
        Task<TransmissionTypeDto>              CreateAsync(CreateTransmissionTypeRequest request);
        Task<bool>                             UpdateAsync(int id, UpdateTransmissionTypeRequest request);
        Task<bool>                             DeleteAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;

namespace Application.Contracts.Repositories
{
    public interface ITransmissionTypeRepository
    {
        Task<TransmissionType?>            GetByIdAsync(int id);
        Task<IEnumerable<TransmissionType>> GetAllAsync();
        Task<bool>                         ExistsByNameAsync(string name);
        Task                               AddAsync(TransmissionType transmissionType);
        void                               Update(TransmissionType transmissionType);
        void                               Remove(TransmissionType transmissionType);
    }
}
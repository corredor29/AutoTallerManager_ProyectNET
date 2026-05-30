using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PhoneCodes;
using Application.Requests.PhoneCodes;

namespace Application.Contracts.Services
{
    public interface IPhoneCodeService
    {
        Task<IEnumerable<PhoneCodeDto>> GetAllAsync();
        Task<PhoneCodeDto?>             GetByIdAsync(int id);
        Task<PhoneCodeDto>              CreateAsync(CreatePhoneCodeRequest request);
        Task<bool>                      UpdateAsync(int id, UpdatePhoneCodeRequest request);
        Task<bool>                      DeleteAsync(int id);
    }
}
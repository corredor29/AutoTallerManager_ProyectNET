using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Users;
using Application.Requests.Users;

namespace Application.Contracts.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?>  GetByIdAsync(int id);
        Task<UserDto>   CreateAsync(CreateUserRequest request);
        Task<bool>  UpdateAsync(int id, UpdateUserRequest request);
        Task<bool>  DeleteAsync(int id);
        Task<bool>  ActivateAsync(int id);
        Task<bool>   DeactivateAsync(int id);
    }
}
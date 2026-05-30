using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Roles;
using Application.Requests.Roles;

namespace Application.Contracts.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?>             GetByIdAsync(int id);
        Task<RoleDto>              CreateAsync(CreateRoleRequest request);
        Task<bool>                 UpdateAsync(int id, UpdateRoleRequest request);
        Task<bool>                 DeleteAsync(int id);
    }
}
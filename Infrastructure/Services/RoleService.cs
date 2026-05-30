using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Roles;
using Application.Requests.Roles;
using Domain.Entities.Users;
using Domain.ValueObject.Users.Role;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class RoleService : IRoleService
    {
        private readonly IRoleRepository     _roleRepository;
        private readonly AutoTallerDbContext _dbContext;

        public RoleService(IRoleRepository roleRepository,
                        AutoTallerDbContext dbContext)
        {
            _roleRepository = roleRepository;
            _dbContext      = dbContext;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(MapToDto);
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role is null ? null : MapToDto(role);
        }

        public async Task<RoleDto> CreateAsync(CreateRoleRequest request)
        {
            await EnsureRoleNameIsUniqueAsync(request.RoleName);

            var role = new Role(new RoleName(request.RoleName));

            await _roleRepository.AddAsync(role);
            await _dbContext.SaveChangesAsync();

            return MapToDto(role);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null) return false;

            await EnsureRoleNameIsUniqueAsync(request.RoleName, id);

            role.Update(new RoleName(request.RoleName));

            _roleRepository.Update(role);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null) return false;

            if (await _dbContext.UserRoles.AnyAsync(x => x.RoleId == id))
                throw new InvalidOperationException($"Role {id} is being used and cannot be deleted.");

            _roleRepository.Remove(role);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task EnsureRoleNameIsUniqueAsync(string roleName, int? excludeId = null)
        {
            var normalizedName = roleName.Trim().ToLower();
            var exists = await _dbContext.Roles.AnyAsync(x =>
                x.RoleName.Value.ToLower() == normalizedName &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new InvalidOperationException($"Role '{roleName}' already exists.");
        }

        private static RoleDto MapToDto(Role role) => new()
        {
            Id       = role.Id,
            RoleName = role.RoleName.Value
        };
    }
}
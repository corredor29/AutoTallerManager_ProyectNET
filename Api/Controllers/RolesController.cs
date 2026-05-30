using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.Roles;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role is null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequest request)
        {
            var updated = await _roleService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _roleService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
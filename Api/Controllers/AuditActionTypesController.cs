using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.AuditActionTypes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class AuditActionTypesController : ControllerBase
    {
        private readonly IAuditActionTypeService _auditActionTypeService;

        public AuditActionTypesController(IAuditActionTypeService auditActionTypeService)
        {
            _auditActionTypeService = auditActionTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actionTypes = await _auditActionTypeService.GetAllAsync();
            return Ok(actionTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var actionType = await _auditActionTypeService.GetByIdAsync(id);
            if (actionType is null)
                return NotFound();

            return Ok(actionType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditActionTypeRequest request)
        {
            var actionType = await _auditActionTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = actionType.Id }, actionType);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuditActionTypeRequest request)
        {
            var updated = await _auditActionTypeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _auditActionTypeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
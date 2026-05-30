using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.TransmissionTypes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
    public sealed class TransmissionTypesController : ControllerBase
    {
        private readonly ITransmissionTypeService _transmissionTypeService;

        public TransmissionTypesController(ITransmissionTypeService transmissionTypeService)
        {
            _transmissionTypeService = transmissionTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transmissionTypes = await _transmissionTypeService.GetAllAsync();
            return Ok(transmissionTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transmissionType = await _transmissionTypeService.GetByIdAsync(id);
            if (transmissionType is null)
                return NotFound();

            return Ok(transmissionType);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateTransmissionTypeRequest request)
        {
            var transmissionType = await _transmissionTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = transmissionType.Id }, transmissionType);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTransmissionTypeRequest request)
        {
            var updated = await _transmissionTypeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _transmissionTypeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
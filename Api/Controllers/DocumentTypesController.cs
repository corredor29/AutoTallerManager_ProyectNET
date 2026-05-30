using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.DocumentTypes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class DocumentTypesController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var documentTypes = await _documentTypeService.GetAllAsync();
            return Ok(documentTypes);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var documentType = await _documentTypeService.GetByIdAsync(id);
            if (documentType is null)
                return NotFound();

            return Ok(documentType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeRequest request)
        {
            var documentType = await _documentTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = documentType.Id }, documentType);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeRequest request)
        {
            var updated = await _documentTypeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _documentTypeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
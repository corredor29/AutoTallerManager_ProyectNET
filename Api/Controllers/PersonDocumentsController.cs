using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.PersonDocuments;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
    public sealed class PersonDocumentsController : ControllerBase
    {
        private readonly IPersonDocumentService _personDocumentService;

        public PersonDocumentsController(IPersonDocumentService personDocumentService)
        {
            _personDocumentService = personDocumentService;
        }

        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetByPersonId(int personId)
        {
            var documents = await _personDocumentService.GetByPersonIdAsync(personId);
            return Ok(documents);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var document = await _personDocumentService.GetByIdAsync(id);
            if (document is null)
                return NotFound();

            return Ok(document);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreatePersonDocumentRequest request)
        {
            var document = await _personDocumentService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonDocumentRequest request)
        {
            var updated = await _personDocumentService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id:int}/primary")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> SetAsPrimary(int id)
        {
            var updated = await _personDocumentService.SetAsPrimaryAsync(id);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _personDocumentService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.Persons;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var persons = await _personService.GetAllAsync();
            return Ok(persons);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person is null)
                return NotFound();

            return Ok(person);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
        {
            var person = await _personService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonRequest request)
        {
            var updated = await _personService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _personService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
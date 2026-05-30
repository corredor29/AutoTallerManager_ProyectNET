using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.PersonPhones;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
    public sealed class PersonPhonesController : ControllerBase
    {
        private readonly IPersonPhoneService _personPhoneService;

        public PersonPhonesController(IPersonPhoneService personPhoneService)
        {
            _personPhoneService = personPhoneService;
        }

        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetByPersonId(int personId)
        {
            var phones = await _personPhoneService.GetByPersonIdAsync(personId);
            return Ok(phones);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phone = await _personPhoneService.GetByIdAsync(id);
            if (phone is null)
                return NotFound();

            return Ok(phone);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreatePersonPhoneRequest request)
        {
            var phone = await _personPhoneService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = phone.Id }, phone);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonPhoneRequest request)
        {
            var updated = await _personPhoneService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id:int}/primary")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> SetAsPrimary(int id)
        {
            var updated = await _personPhoneService.SetAsPrimaryAsync(id);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _personPhoneService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
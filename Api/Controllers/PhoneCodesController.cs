using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.PhoneCodes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class PhoneCodesController : ControllerBase
    {
        private readonly IPhoneCodeService _phoneCodeService;

        public PhoneCodesController(IPhoneCodeService phoneCodeService)
        {
            _phoneCodeService = phoneCodeService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var phoneCodes = await _phoneCodeService.GetAllAsync();
            return Ok(phoneCodes);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var phoneCode = await _phoneCodeService.GetByIdAsync(id);
            if (phoneCode is null)
                return NotFound();

            return Ok(phoneCode);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePhoneCodeRequest request)
        {
            var phoneCode = await _phoneCodeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = phoneCode.Id }, phoneCode);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePhoneCodeRequest request)
        {
            var updated = await _phoneCodeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _phoneCodeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
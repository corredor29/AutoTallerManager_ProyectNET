using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.PersonEmails;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo PersonEmails.
    public sealed class PersonEmailsController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IPersonEmailService _personEmailService;

        public PersonEmailsController(IPersonEmailService personEmailService)
        {
            _personEmailService = personEmailService;
        }

    // Obtiene los registros asociados a una persona especifica.
        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetByPersonId(int personId)
        {
            var emails = await _personEmailService.GetByPersonIdAsync(personId);
            return Ok(emails);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var email = await _personEmailService.GetByIdAsync(id);
            if (email is null)
                return NotFound();

            return Ok(email);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Create([FromBody] CreatePersonEmailRequest request)
        {
            var email = await _personEmailService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = email.Id }, email);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonEmailRequest request)
        {
            var updated = await _personEmailService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Marca este registro relacionado como el principal para la persona.
        [HttpPut("{id:int}/primary")]
        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        public async Task<IActionResult> SetAsPrimary(int id)
        {
            var updated = await _personEmailService.SetAsPrimaryAsync(id);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _personEmailService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

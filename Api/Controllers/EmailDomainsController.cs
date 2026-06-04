using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Requests.EmailDomains;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
// Controlador que expone los endpoints principales del modulo EmailDomains.
    public sealed class EmailDomainsController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IEmailDomainService _emailDomainService;

        public EmailDomainsController(IEmailDomainService emailDomainService)
        {
            _emailDomainService = emailDomainService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var domains = await _emailDomainService.GetAllAsync();
            return Ok(domains);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var domain = await _emailDomainService.GetByIdAsync(id);
            if (domain is null)
                return NotFound();

            return Ok(domain);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmailDomainRequest request)
        {
            var domain = await _emailDomainService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = domain.Id }, domain);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmailDomainRequest request)
        {
            var updated = await _emailDomainService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _emailDomainService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

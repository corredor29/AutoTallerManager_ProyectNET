using Application.Contracts.Services;
using Application.Requests.DocumentTypes;
using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
// Controlador que expone los endpoints principales del modulo DocumentTypes.
    public sealed class DocumentTypesController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetAll()
        {
            var documentTypes = await _documentTypeService.GetAllAsync();
            return Ok(documentTypes);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        [Authorize(Roles = AppRoles.Staff)]
        public async Task<IActionResult> GetById(int id)
        {
            var documentType = await _documentTypeService.GetByIdAsync(id);
            if (documentType is null)
                return NotFound();

            return Ok(documentType);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeRequest request)
        {
            var documentType = await _documentTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = documentType.Id }, documentType);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeRequest request)
        {
            var updated = await _documentTypeService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _documentTypeService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

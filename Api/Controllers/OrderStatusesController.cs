using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.OrderStatuses;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
// Controlador que expone los endpoints principales del modulo OrderStatuses.
    public sealed class OrderStatusesController : ControllerBase
    {
    // Servicio que contiene la logica de negocio usada por este controlador.
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusesController(IOrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

    // Obtiene la lista completa del recurso manejado por este controlador.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var statuses = await _orderStatusService.GetAllAsync();
            return Ok(statuses);
        }

    // Busca un registro puntual por su identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var status = await _orderStatusService.GetByIdAsync(id);
            if (status is null)
                return NotFound();

            return Ok(status);
        }

    // Crea un nuevo registro a partir de los datos enviados en el body.
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateOrderStatusRequest request)
        {
            var status = await _orderStatusService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
        }

    // Actualiza un registro existente identificado por su id.
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var updated = await _orderStatusService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

    // Elimina el registro indicado si existe.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _orderStatusService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}

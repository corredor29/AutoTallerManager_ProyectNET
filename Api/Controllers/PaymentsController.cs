using Application.Contracts.Services;
using Application.Requests.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo Payments.
public sealed class PaymentsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment is null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        var payment = await _paymentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentRequest request)
    {
        var updated = await _paymentService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Elimina el registro indicado si existe.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _paymentService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

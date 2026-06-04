using Application.Contracts.Services;
using Application.Requests.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// Controlador que expone los endpoints principales del modulo PaymentMethods.
public sealed class PaymentMethodsController : ControllerBase
{
    // Servicio que contiene la logica de negocio usada por este controlador.
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodsController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    // Obtiene la lista completa del recurso manejado por este controlador.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var methods = await _paymentMethodService.GetAllAsync();
        return Ok(methods);
    }

    // Busca un registro puntual por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var method = await _paymentMethodService.GetByIdAsync(id);
        if (method is null)
        {
            return NotFound();
        }

        return Ok(method);
    }

    // Crea un nuevo registro a partir de los datos enviados en el body.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentMethodRequest request)
    {
        var method = await _paymentMethodService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = method.Id }, method);
    }

    // Actualiza un registro existente identificado por su id.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentMethodRequest request)
    {
        var updated = await _paymentMethodService.UpdateAsync(id, request);
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
        var removed = await _paymentMethodService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

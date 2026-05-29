using Application.Contracts.Services;
using Application.Requests.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodsController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var methods = await _paymentMethodService.GetAllAsync();
        return Ok(methods);
    }

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentMethodRequest request)
    {
        var method = await _paymentMethodService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = method.Id }, method);
    }

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

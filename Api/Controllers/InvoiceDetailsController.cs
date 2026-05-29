using Application.Contracts.Services;
using Application.Requests.InvoiceDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InvoiceDetailsController : ControllerBase
{
    private readonly IInvoiceDetailService _invoiceDetailService;

    public InvoiceDetailsController(IInvoiceDetailService invoiceDetailService)
    {
        _invoiceDetailService = invoiceDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _invoiceDetailService.GetAllAsync();
        return Ok(details);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _invoiceDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDetailRequest request)
    {
        var detail = await _invoiceDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceDetailRequest request)
    {
        var updated = await _invoiceDetailService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _invoiceDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

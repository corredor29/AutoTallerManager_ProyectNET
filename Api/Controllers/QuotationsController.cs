using Application.Contracts.Services;
using Application.Requests.Quotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuotationsController : ControllerBase
{
    private readonly IQuotationService _quotationService;

    public QuotationsController(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var quotations = await _quotationService.GetAllAsync();
        return Ok(quotations);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var quotation = await _quotationService.GetByIdAsync(id);
        if (quotation is null)
        {
            return NotFound();
        }

        return Ok(quotation);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationRequest request)
    {
        var quotation = await _quotationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = quotation.Id }, quotation);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationRequest request)
    {
        var updated = await _quotationService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeQuotationStatusRequest request)
    {
        var updated = await _quotationService.ChangeStatusAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _quotationService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

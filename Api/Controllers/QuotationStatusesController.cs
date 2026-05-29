using Application.Contracts.Services;
using Application.Requests.QuotationStatuses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuotationStatusesController : ControllerBase
{
    private readonly IQuotationStatusService _quotationStatusService;

    public QuotationStatusesController(IQuotationStatusService quotationStatusService)
    {
        _quotationStatusService = quotationStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _quotationStatusService.GetAllAsync();
        return Ok(statuses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _quotationStatusService.GetByIdAsync(id);
        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationStatusRequest request)
    {
        var status = await _quotationStatusService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationStatusRequest request)
    {
        var updated = await _quotationStatusService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _quotationStatusService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

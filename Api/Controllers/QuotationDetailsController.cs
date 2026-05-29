using Application.Contracts.Services;
using Application.Requests.QuotationDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuotationDetailsController : ControllerBase
{
    private readonly IQuotationDetailService _quotationDetailService;

    public QuotationDetailsController(IQuotationDetailService quotationDetailService)
    {
        _quotationDetailService = quotationDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var details = await _quotationDetailService.GetAllAsync();
        return Ok(details);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _quotationDetailService.GetByIdAsync(id);
        if (detail is null)
        {
            return NotFound();
        }

        return Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuotationDetailRequest request)
    {
        var detail = await _quotationDetailService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationDetailRequest request)
    {
        var updated = await _quotationDetailService.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removed = await _quotationDetailService.DeleteAsync(id);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.MileageHistory;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Staff)]
    public sealed class MileageHistoryController : ControllerBase
    {
        private readonly IMileageHistoryService _mileageHistoryService;

        public MileageHistoryController(IMileageHistoryService mileageHistoryService)
        {
            _mileageHistoryService = mileageHistoryService;
        }

        [HttpGet("vehicle/{vehicleId:int}")]
        public async Task<IActionResult> GetByVehicleId(int vehicleId)
        {
            var mileageHistories = await _mileageHistoryService.GetByVehicleIdAsync(vehicleId);
            return Ok(mileageHistories);
        }

        [HttpGet("vehicle/{vehicleId:int}/latest")]
        public async Task<IActionResult> GetLatestByVehicleId(int vehicleId)
        {
            var mileageHistory = await _mileageHistoryService.GetLatestByVehicleIdAsync(vehicleId);
            if (mileageHistory is null)
                return NotFound();

            return Ok(mileageHistory);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mileageHistory = await _mileageHistoryService.GetByIdAsync(id);
            if (mileageHistory is null)
                return NotFound();

            return Ok(mileageHistory);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrMechanic)]
        public async Task<IActionResult> Create([FromBody] CreateMileageHistoryRequest request)
        {
            var mileageHistory = await _mileageHistoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = mileageHistory.Id }, mileageHistory);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.AdminOrMechanic)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMileageHistoryRequest request)
        {
            var updated = await _mileageHistoryService.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _mileageHistoryService.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
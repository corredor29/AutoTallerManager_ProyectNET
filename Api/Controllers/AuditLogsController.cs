using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Services;
using Application.Requests.AuditLogs;
using Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _auditLogService.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _auditLogService.GetByIdAsync(id);
            if (log is null)
                return NotFound();

            return Ok(log);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var logs = await _auditLogService.GetByUserIdAsync(userId);
            return Ok(logs);
        }

        [HttpGet("entity/{entityName}")]
        public async Task<IActionResult> GetByEntity(string entityName)
        {
            var logs = await _auditLogService.GetByEntityAsync(entityName);
            return Ok(logs);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var logs = await _auditLogService.GetByDateRangeAsync(from, to);
            return Ok(logs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditLogRequest request)
        {
            var log = await _auditLogService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = log.Id }, log);
        }
    }
}
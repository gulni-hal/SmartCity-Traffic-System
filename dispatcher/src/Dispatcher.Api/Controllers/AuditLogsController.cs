using Dispatcher.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispatcher.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogRepository _repository;

    public AuditLogsController(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Authorize] // Sadece giriş yapanlar (admin) görebilsin
    public async Task<IActionResult> GetRecentLogs()
    {
        // En son atılan 50 isteği getir
        var logs = await _repository.GetRecentLogsAsync(50);
        return Ok(logs);
    }
}
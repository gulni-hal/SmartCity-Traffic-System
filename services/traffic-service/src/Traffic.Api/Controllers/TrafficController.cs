using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Traffic.Application.Services;

namespace Traffic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrafficController : ControllerBase
{
    private readonly TrafficService _trafficService;

    public TrafficController(TrafficService trafficService)
    {
        _trafficService = trafficService;
    }

    [HttpPost("live")]
    public async Task<IActionResult> RecordTraffic([FromBody] TrafficRecordRequest request)
    {
        var result = await _trafficService.RecordTrafficAsync(request);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}
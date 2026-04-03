using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Traffic.Application.Interfaces;
using Traffic.Application.DTOs;

namespace Traffic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrafficController : ControllerBase
{
    private readonly ITrafficService _trafficService;

    // HOCA UYUMU: Controller artık Interface'e bağımlı.
    public TrafficController(ITrafficService trafficService)
    {
        _trafficService = trafficService;
    }

    [HttpPost("live")]
    public async Task<IActionResult> RecordTraffic([FromBody] TrafficRecordRequest request)
    {
        var result = await _trafficService.RecordTrafficAsync(request);

        if (result.Success)
        {
            return Created($"/api/traffic/{request.LocationId}", result);
        }

        return BadRequest(new { Error = result.ErrorMessage });
    }

    [HttpGet("{locationId}")]
    public async Task<IActionResult> GetTraffic(string locationId)
    {
        var records = await _trafficService.GetTrafficByLocationAsync(locationId);

        if (records == null || !records.Any())
        {
            return NotFound(new { Message = "Trafik verisi bulunamadı." });
        }

        return Ok(records);
    }

    [HttpGet("hotspots")]
    public async Task<IActionResult> GetHotspots()
    {
        var records = await _trafficService.GetHotspotsAsync();
        return Ok(records);
    }

}
using System.Linq;
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

    // RMM SEVİYE 2 UYUMU: GET Metodu ve URL'den okunan parametre
    // Örnek İstek: GET /api/traffic/Kadikoy-Merkez-1
    [HttpGet("{locationId}")]
    public async Task<IActionResult> GetTraffic(string locationId)
    {
        var records = await _trafficService.GetTrafficByLocationAsync(locationId);

        if (records == null || !records.Any())
        {
            // Kayıt yoksa 404 dönüyoruz
            return NotFound(new { Message = $"{locationId} lokasyonuna ait trafik verisi bulunamadı." });
        }

        // Kayıt varsa 200 OK ile JSON formatında dönüyoruz
        return Ok(records);
    }
}
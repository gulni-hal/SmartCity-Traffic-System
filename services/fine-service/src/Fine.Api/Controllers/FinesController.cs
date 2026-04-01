using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;

namespace Fine.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Çıktı: /api/fines
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;

    public FinesController(IFineService fineService)
    {
        _fineService = fineService;
    }

    // CREATE İŞLEMİ -> POST Metodu -> Başarılıysa 200 veya 201 döner
    [HttpPost]
    public async Task<IActionResult> CreateFine([FromBody] CreateFineRequest request)
    {
        var result = await _fineService.CreateFineAsync(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(new { Error = "Ceza kaydı oluşturulamadı." });
    }

    // READ İŞLEMİ -> GET Metodu -> URL'den parametre alır (RMM Seviye 2 Uyumu)
    // Örnek İstek: GET /api/fines/34ABC123
    [HttpGet("{licensePlate}")]
    public async Task<IActionResult> GetFines(string licensePlate)
    {
        var fines = await _fineService.GetFinesByPlateAsync(licensePlate);

        if (fines == null || !fines.Any())
        {
            // Veri yoksa RMM Seviye 2 gereği 404 Not Found dönmelidir!
            return NotFound(new { Message = $"{licensePlate} plakasına ait ceza bulunamadı." });
        }

        // Veri varsa 200 OK ile JSON dönmelidir
        return Ok(fines);
    }
}
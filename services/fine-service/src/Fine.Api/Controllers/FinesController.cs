using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fine.Application.DTOs;
using Fine.Application.Interfaces;

namespace Fine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;

    public FinesController(IFineService fineService)
    {
        _fineService = fineService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateFine([FromBody] CreateFineRequest request)
    {
        var result = await _fineService.CreateFineAsync(request);

        if (result.Success)
        {
            FineMetrics.FineCreated.Inc();
            return Created($"/api/fines/{request.LicensePlate}", result);
        }

        FineMetrics.FineValidationFailed.Inc();
        return BadRequest(new { Error = "Ceza kaydı oluşturulamadı. Lütfen bilgileri kontrol edin." });
    }

    [HttpGet("{licensePlate}")]
    public async Task<IActionResult> GetFines(string licensePlate)
    {
        var fines = await _fineService.GetFinesByPlateAsync(licensePlate);

        if (fines == null || !fines.Any())
        {
            return NotFound(new { Message = $"{licensePlate} plakasına ait ceza bulunamadı." });
        }

        return Ok(fines);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFines()
    {
        var fines = await _fineService.GetAllFinesAsync();
        return Ok(fines);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFine(string id)
    {
        var result = await _fineService.DeleteFineAsync(id);

        if (!result)
        {
            return NotFound(new { Message = "Silinecek ceza kaydı bulunamadı." });
        }

        FineMetrics.FineDeleted.Inc();
        return NoContent();
    }
}

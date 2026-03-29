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
            return Ok(result);
        }

        return BadRequest(new { Error = "Ceza kaydı oluşturulamadı." });
    }
}
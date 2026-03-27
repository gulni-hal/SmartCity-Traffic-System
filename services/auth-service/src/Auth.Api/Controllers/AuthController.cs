using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    //Dependency Injection sayesinde IAuthService otomatik olarak buraya gelecek
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        //Gelen JSON verisi otomatik olarak RegisterRequest nesnesine donusturulur.
        // Servisimizdeki is mantigini cagiriyoruz.
        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            //İsterlerde belirtildigi gibi sonucu JSON olarak ve 200 OK ile donuyoruz
            return Ok(result);
        }

        //eger basarisiz olsaydi 400 Bad Request donebilirdik(ileride eklenecek)
        return BadRequest(new { Error = "Kayıt işlemi başarısız oldu." });
    }
}
using Microsoft.AspNetCore.Mvc;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            AuthMetrics.RegisterSuccess.Inc();
            return Created(string.Empty, result);
        }

        return BadRequest(new { Error = "Kayıt işlemi başarısız oldu." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result.Success)
        {
            AuthMetrics.LoginSuccess.Inc();
            return Ok(result);
        }

        AuthMetrics.LoginFailed.Inc();
        return Unauthorized(new { Error = "Kullanıcı adı veya şifre hatalı." });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateTokenRequest request)
    {
        var result = await _authService.ValidateTokenAsync(request.Token);

        if (!result.IsValid)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _authService.GetUserAsync(username);

        if (user == null)
        {
            return NotFound(new { Message = "Kullanıcı bulunamadı." });
        }

        return Ok(user);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Unauthorized(new { Error = "Authorization header eksik." });
        }

        var token = ExtractToken(authHeader.ToString());

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new { Error = "Geçersiz token." });
        }

        var result = await _authService.LogoutAsync(token);

        if (!result)
        {
            return Unauthorized(new { Error = "Çıkış işlemi başarısız." });
        }

        AuthMetrics.LogoutSuccess.Inc();
        return Ok(new { Message = "Başarıyla çıkış yapıldı." });
    }

    private static string ExtractToken(string authorizationHeader)
    {
        const string bearerPrefix = "Bearer ";

        if (authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader[bearerPrefix.Length..].Trim();
        }

        return string.Empty;
    }
}

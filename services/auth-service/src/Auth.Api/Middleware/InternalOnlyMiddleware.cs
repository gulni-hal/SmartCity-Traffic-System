using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Auth.Api.Middleware;

public class InternalOnlyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public InternalOnlyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // appsettings.json'dan beklediğimiz şifreyi çekiyoruz
        var expectedSecret = _configuration["InternalSecret"];

        // İstek Dispatcher'dan gelmiyorsa (şifre yoksa veya yanlışsa) kapıdan çevir
        if (!context.Request.Headers.TryGetValue("X-Internal-Secret", out var providedSecret) || providedSecret != expectedSecret)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Erisim reddedildi: Bu servise sadece Dispatcher uzerinden ulasilabilir.");
            return;
        }

        // Şifre doğruysa içeri al
        await _next(context);
    }
}
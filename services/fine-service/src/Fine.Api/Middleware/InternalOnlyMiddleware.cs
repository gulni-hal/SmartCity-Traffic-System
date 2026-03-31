using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Fine.Api.Middleware;

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
        // appsettings.json'dan beklenen gizli şifreyi alıyoruz
        var expectedSecret = _configuration["InternalSecret"];

        // Gelen istekte "X-Internal-Secret" başlığı var mı ve değeri bizimkiyle eşleşiyor mu kontrol ediyoruz
        if (!context.Request.Headers.TryGetValue("X-Internal-Secret", out var providedSecret) || providedSecret != expectedSecret)
        {
            // Eşleşmiyorsa (veya hiç yoksa) 403 Forbidden dön ve isteği burada kes (Kapıdan çevir)
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Erisim reddedildi: Bu servise sadece Dispatcher uzerinden ulasilabilir.");
            return;
        }

        // Şifre doğruysa (Dispatcher'dan geliyorsa) isteğin servise gitmesine izin ver
        await _next(context);
    }
}
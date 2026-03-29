using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dispatcher.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // İsteği normal akışında bir sonraki adıma (YARP'a) bırakıyoruz
            await _next(context);

            // Eğer arkadaki mikroservis kapalıysa veya hata dönerse (örn: 502 Bad Gateway)
            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
            {
                _logger.LogWarning("HTTP {StatusCode} hatası oluştu. İstek atılan adres: {Path}",
                    context.Response.StatusCode, context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            // Sistemde beklenmeyen bir Exception (çökme) patlarsa burası yakalar
            _logger.LogError(ex, "Sistemde kritik bir hata yakalandı! Adres: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Testin bizden beklediği 3 ana şartı (JSON, 500 kodu ve Success:false) sağlıyoruz
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 kodu

        var response = new
        {
            Success = false,
            Error = "Sistemde beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
            Message = exception.Message // Geliştirici ortamı için hatanın detayını da ekliyoruz
        };

        // Oluşturduğumuz nesneyi JSON metnine çevirip kullanıcıya dönüyoruz
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
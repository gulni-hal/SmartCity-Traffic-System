using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dispatcher.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Kronometreyi başlatıyoruz
        var sw = Stopwatch.StartNew();
        _logger.LogInformation(">>> GELEN İSTEK: {Method} {Path}", context.Request.Method, context.Request.Path);

        // İsteği diğer middleware'lere (Auth, YARP vs.) iletiyoruz
        await _next(context);

        // Kronometreyi durdurup geçen süreyi ve sonucu logluyoruz
        sw.Stop();
        _logger.LogInformation("<<< GİDEN YANIT: {Method} {Path} | Durum Kodu: {StatusCode} | Süre: {Elapsed}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
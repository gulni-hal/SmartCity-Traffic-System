using System.Diagnostics;
using Dispatcher.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dispatcher.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(">>> GELEN İSTEK: {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        sw.Stop();

        _logger.LogInformation("<<< GİDEN YANIT: {Method} {Path} | Durum Kodu: {StatusCode} | Süre: {Elapsed}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);

        var auditLog = new RequestAuditLog
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            StatusCode = context.Response.StatusCode,
            Username = context.Items["Username"]?.ToString() ?? string.Empty,
            Role = context.Items["Role"]?.ToString() ?? string.Empty,
            TargetService = ResolveTargetService(context.Request.Path),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await auditLogRepository.CreateAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit log veritabanına yazılamadı. Path: {Path}", context.Request.Path);
        }
    }

    private static string ResolveTargetService(PathString path)
    {
        if (path.StartsWithSegments("/api/auth")) return "auth-service";
        if (path.StartsWithSegments("/api/traffic")) return "traffic-service";
        if (path.StartsWithSegments("/api/fines")) return "fine-service";
        if (path.StartsWithSegments("/health")) return "dispatcher";

        return "unknown";
    }
}

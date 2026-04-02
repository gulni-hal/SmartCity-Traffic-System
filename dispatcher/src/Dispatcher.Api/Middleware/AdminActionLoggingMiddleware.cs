using Dispatcher.Application;

namespace Dispatcher.Api.Middleware;

public class AdminActionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminActionLoggingMiddleware> _logger;

    public AdminActionLoggingMiddleware(
        RequestDelegate next,
        ILogger<AdminActionLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAdminActionLogRepository repository)
    {
        await _next(context);

        var role = context.Items["Role"]?.ToString() ?? string.Empty;
        var username = context.Items["Username"]?.ToString() ?? string.Empty;

        if (role != "Admin")
        {
            return;
        }

        if (!IsAdminAction(context.Request.Method, context.Request.Path))
        {
            return;
        }

        var log = new AdminActionLog
        {
            Username = username,
            Role = role,
            Method = context.Request.Method,
            Path = context.Request.Path,
            ActionName = ResolveActionName(context.Request.Method, context.Request.Path),
            TargetService = ResolveTargetService(context.Request.Path),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await repository.CreateAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin action log yazilamadi. Path: {Path}", context.Request.Path);
        }
    }

    private static bool IsAdminAction(string method, PathString path)
    {
        if (method is "POST" or "PUT" or "PATCH" or "DELETE")
        {
            return path.StartsWithSegments("/api/fines") ||
                   path.StartsWithSegments("/api/traffic");
        }

        return false;
    }

    private static string ResolveActionName(string method, PathString path)
    {
        return $"{method} {path}";
    }

    private static string ResolveTargetService(PathString path)
    {
        if (path.StartsWithSegments("/api/traffic")) return "traffic-service";
        if (path.StartsWithSegments("/api/fines")) return "fine-service";
        return "unknown";
    }
}

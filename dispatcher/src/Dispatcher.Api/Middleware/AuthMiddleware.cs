using Dispatcher.Application;

namespace Dispatcher.Api.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthValidationService authValidationService)
    {
        var path = context.Request.Path;

        if (IsPublicPath(path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            await WriteUnauthorizedAsync(context);
            return;
        }

        var token = authHeader.ToString().Replace("Bearer ", "").Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            await WriteUnauthorizedAsync(context);
            return;
        }

        var validationResult = await authValidationService.ValidateAsync(token);

        if (!validationResult.IsValid)
        {
            await WriteUnauthorizedAsync(context);
            return;
        }

        context.Items["Username"] = validationResult.Username;
        context.Items["Role"] = validationResult.Role;

        await _next(context);
    }

    private static bool IsPublicPath(PathString path)
    {
        return path.StartsWithSegments("/health") ||
               path.StartsWithSegments("/api/auth");
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}

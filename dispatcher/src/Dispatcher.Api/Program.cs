using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Dispatcher.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5154");
});

builder.Services.AddScoped<IAuthValidationService, AuthValidationService>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AuthMiddleware>();

app.MapReverseProxy();

app.Run();

public partial class Program { }

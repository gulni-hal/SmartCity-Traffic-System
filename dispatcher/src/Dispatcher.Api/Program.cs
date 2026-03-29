using Dispatcher.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// YARP ekle
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<AuthMiddleware>();

// Reverse proxy baþlat
app.MapReverseProxy();

app.Run();

public partial class Program { }
using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Dispatcher.Infrastructure;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

var authServiceBaseUrl = builder.Configuration["AuthService:BaseUrl"];
var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];
var internalSecret = builder.Configuration["InternalSecret"];

builder.Services.AddScoped<IAuditLogRepository>(provider =>
    new MongoAuditLogRepository(mongoConnectionString!, mongoDatabaseName!)
);

builder.Services.AddScoped<IAdminActionLogRepository>(provider =>
    new MongoAdminActionLogRepository(mongoConnectionString!, mongoDatabaseName!)
);




builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(authServiceBaseUrl!);
    client.DefaultRequestHeaders.Add("X-Internal-Secret", internalSecret!);
});


builder.Services.AddScoped<IAuthValidationService, AuthValidationService>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin() // Herkese açık (Geliştirme ortamı için ideal)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Healthy"));

// 1. Prometheus metrik toplamaya en üstte başlasın
app.UseHttpMetrics();
app.UseCors("AllowFrontend");
app.Use(async (context, next) =>
{
    if (HttpMethods.IsOptions(context.Request.Method))
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return;
    }

    await next();
});

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// 2. Prometheus'un verileri okuduğu ucu YETKİLENDİRMEDEN ÖNCE açıyoruz!
app.MapMetrics();

// 3. Bizim sıkı güvenlik görevlimiz (Sadece yetki gerektiren sayfalarda çalışacak)
app.UseWhen(context =>
    !context.Request.Path.StartsWithSegments("/metrics") &&
    !context.Request.Path.StartsWithSegments("/health"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<AuthMiddleware>();
    }
);

app.MapReverseProxy();

app.Run();

public partial class Program { }

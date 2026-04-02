using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Dispatcher.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

var authServiceBaseUrl = builder.Configuration["AuthService:BaseUrl"];
var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

builder.Services.AddScoped<IAdminActionLogRepository>(provider =>
    new MongoAdminActionLogRepository(mongoConnectionString!, mongoDatabaseName!)
);



builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(authServiceBaseUrl!);
});


builder.Services.AddScoped<IAuthValidationService, AuthValidationService>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));



var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseMiddleware<AdminActionLoggingMiddleware>();


app.MapReverseProxy();

app.Run();

public partial class Program { }

using Auth.Application.Interfaces;
using Auth.Application.Services;
using Auth.Infrastructure.Repositories;
using Auth.Api.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

builder.Services.AddScoped<IUserRepository>(provider =>
    new MongoUserRepository(mongoConnectionString!, mongoDatabaseName!)
);

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseWhen(context =>
    !context.Request.Path.StartsWithSegments("/metrics") &&
    !context.Request.Path.StartsWithSegments("/health"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<InternalOnlyMiddleware>();
    });

app.MapControllers();

app.Run();

using Traffic.Application.Interfaces;
using Traffic.Application.Services;
using Traffic.Infrastructure.Repositories;
using Traffic.Api.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

builder.Services.AddScoped<ITrafficRepository>(provider =>
    new MongoTrafficRepository(mongoConnectionString!, mongoDatabaseName!)
);
builder.Services.AddScoped<ITrafficService, TrafficService>();

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

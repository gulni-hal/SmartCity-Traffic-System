using Fine.Application.Interfaces;
using Fine.Application.Services;
using Fine.Infrastructure.Repositories;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

builder.Services.AddScoped<IFineRepository>(provider =>
    new MongoFineRepository(mongoConnectionString!, mongoDatabaseName!)
);

builder.Services.AddScoped<IFineService, FineService>();

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
        appBuilder.UseMiddleware<Fine.Api.Middleware.InternalOnlyMiddleware>();
    });

app.MapControllers();

app.Run();

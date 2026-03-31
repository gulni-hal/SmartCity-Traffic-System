using Fine.Application.Interfaces;
using Fine.Application.Services;
using Fine.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// appsettings.json'dan MongoDB bilgilerini çekiyoruz
var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

// Dependency Injection (DI) Kayıtları
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

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<Fine.Api.Middleware.InternalOnlyMiddleware>();

app.MapControllers();

app.Run();
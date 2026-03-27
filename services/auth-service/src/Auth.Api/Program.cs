using Auth.Application.Interfaces;
using Auth.Application.Services;
using Auth.Infrastructure.Repositories; // Yeni yazdığımız repository için ekledik

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// appsettings.json içindeki MongoDB ayarlarını okuyoruz
var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];

// Dependency Injection (DI) Kayıtları
// IUserRepository istendiğinde, ayarları içine koyarak MongoUserRepository veriyoruz
builder.Services.AddScoped<IUserRepository>(provider =>
    new MongoUserRepository(mongoConnectionString!, mongoDatabaseName!)
);

// AuthService'i ekliyoruz (Artık otomatik olarak MongoUserRepository'yi kullanacak)
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
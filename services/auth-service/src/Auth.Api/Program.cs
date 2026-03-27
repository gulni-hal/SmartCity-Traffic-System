using Auth.Application.Interfaces;
using Auth.Application.Services;

var builder = WebApplication.CreateBuilder(args);

//API Controller'larini projeye ekliyoruz
builder.Services.AddControllers();

//Swagger API dokumantasyonu ve test arayuzu ayarlari
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TDD ile yazdigimiz servisimizi Dependency Injection (DI) konteynerine kaydediyoruz
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

//Gelistirme ortamindaysak Swagger arayuzunu aç
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Guvenlik ve yonlendirme middleware'leri
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
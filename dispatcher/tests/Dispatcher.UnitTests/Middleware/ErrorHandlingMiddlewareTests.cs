using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Dispatcher.Api.Middleware;

namespace Dispatcher.UnitTests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Return_500_And_Json_When_Exception_Thrown()
    {
        // Arrange (Hazırlık)
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream(); // Yanıtın gövdesini okuyabilmek için stream atıyoruz

        // Sistemde beklenmeyen bir hata fırlatan sahte bir rota (next) simüle ediyoruz
        RequestDelegate next = (HttpContext hc) => throw new Exception("Veritabanı çöktü!");

        // Loglama bağımlılığı için testlerde hata vermeyen NullLogger kullanıyoruz
        var logger = new NullLogger<ErrorHandlingMiddleware>();
        var middleware = new ErrorHandlingMiddleware(next, logger);

        // Act (Eylem)
        await middleware.InvokeAsync(context);

        // Assert (Doğrulama)
        // 1. Durum kodu kesinlikle 500 (Internal Server Error) olmalı
        Assert.Equal(500, context.Response.StatusCode);

        // 2. Yanıt tipi düz metin değil, JSON olmalı
        Assert.Equal("application/json", context.Response.ContentType);

        // 3. Yanıtın gövdesini (JSON) okuyup içinde hata mesajı içerip içermediğine bakıyoruz
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.Contains("false", responseText.ToLower()); // Success: false dönmeli
    }
}